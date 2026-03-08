# ADRs — Décisions Architecturales CanBankX

---

## ADR-001 : Style Architectural — Microservices avec DDD

**Statut :** Accepté

### Contexte

CanBankX doit supporter plusieurs domaines métier distincts (identité, comptes, paiements) avec des exigences de conformité réglementaire strictes, des SLAs différenciés par domaine, et une évolution indépendante des équipes.

### Décision

Adopter une **architecture microservices** organisée selon les **Bounded Contexts du Domain-Driven Design (DDD)**, avec trois services autonomes (Users, Accounts, Transactions) partageant une librarie `Shared` contenant uniquement les primitives DDD et l'infrastructure d'authentification commune.

Chaque service suit une **architecture en couches inversées** :
```
Domain (aucune dépendance externe)
  ↑
Application (dépend de Domain)
  ↑
Infrastructure (dépend de Domain + Application)
```

Le pattern **CQRS** est appliqué au sein de chaque service via MediatR.

### Alternatives considérées

| Alternative | Raison du rejet |
|------------|-----------------|
| Architecture monolithique modulaire | Moins adaptée à l'évolution indépendante et aux exigences de disponibilité différenciées |
| Architecture hexagonale pure (ports & adapters) | Patterns DDD jugés plus expressifs pour le domaine bancaire |

### Conséquences

**Positives :**
- Isolation des domaines et des bases de données.
- Déploiements indépendants par service.
- Expressivité du modèle de domaine (agrégats, value objects, domain events).
- Maintenabilité grâce à la séparation claire Domain ↔ Infrastructure.

**Négatives / Risques :**
- Communication synchrone inter-services (latence additionnelle, couplage temporal).
- Transactions distribuées sans saga (risque de cohérence éventuelle).
- Overhead de maintenance de 3 pipelines CI/CD distincts.

---

## ADR-002 : Persistance et Stratégie Transactionnelle

**Statut :** Accepté

### Contexte

Une plateforme bancaire exige une intégrité stricte des données (soldes, transactions), une traçabilité complète et une idempotence des opérations financières. Les contraintes incluent l'isolation des données entre services (Database per Service) et la garantie d'atomicité au sein de chaque service.

### Décision

#### Technologie de persistance

**PostgreSQL 16** (une instance dédiée par service) via **Entity Framework Core 9** avec le pattern **Unit of Work** implémenté directement par le `DbContext`.

Justification du choix PostgreSQL :
- ACID complet, isolation des transactions configurable.
- Support natif UUID, types JSON, index partiels.
- Support officiel EF Core via Npgsql.

#### Pattern Unit of Work avec Domain Events

```csharp
// IUnitOfWork expose SaveEntitiesAsync()
// qui dispatch les domain events AVANT SaveChangesAsync()
public async Task<bool> SaveEntitiesAsync(CancellationToken ct)
{
    await DispatchDomainEventsAsync(ct);  // 1. Dispatch events (MediatR)
    await SaveChangesAsync(ct);           // 2. Persist (PostgreSQL)
    return true;
}
```

**Conséquence :** Les domain events sont dispatché dans la même transaction logique que la persistance de l'agrégat.

#### Idempotence des transactions financières

- Index unique sur `idempotency_key` (contrainte DB) dans la table `transactions`.
- Vérification applicative préalable (`GetByIdempotencyKeyAsync`) avant création.
- Comportement "effectively-once" : la première exécution crée, les suivantes retournent l'existant.

```sql
-- Migration InitialCreate
CREATE UNIQUE INDEX IX_transactions_idempotency_key 
  ON transactions(idempotency_key);
```

#### Mapping des Value Objects

Les Value Objects sont mappés en **Owned Entities** EF Core (colonnes aplaties dans la table de l'agrégat), garantissant :
- Pas de jointures pour les propriétés de valeur.
- Validation à l'instanciation du Value Object.
- Cohérence entre le modèle de domaine et le schéma de base de données.

### Alternatives considérées

| Alternative | Raison du rejet |
|------------|-----------------|
| MongoDB | Transactions multi-documents moins matures, moins adapté au domaine financier |
| Événements persistés après SaveChanges | Risque de perte d'événements si le dispatch échoue |

### Conséquences

**Positives :**
- Atomicité garantie au niveau de chaque service.
- Idempotence des transactions financières par contrainte DB.
- Modèle de domaine riche sans fuites d'infrastructure.

**Négatives / Risques :**
- Transactions distribuées entre services sans saga (risque de cohérence éventuelle si le service Accounts est indisponible lors du dispatch d'un `TransactionCreatedDomainEvent`).
- L'IV AES-256 est statique (configuré en `_env`) — risque de sécurité si la clé est compromise.

---

## ADR-003 : Stratégie d'Erreurs et Versionnage de l'API

**Statut :** Accepté

### Contexte

Une API bancaire doit communiquer des erreurs claires et sécurisées aux clients, sans exposer d'informations sensibles sur l'infrastructure. Elle doit également supporter une évolution sans rupture de contrat.

### Décision

#### Stratégie de gestion des erreurs

**`GlobalExceptionHandler` (`IExceptionHandler` ASP.NET Core)** dans chaque service, retournant des **`ProblemDetails` (RFC 7807)**.

Mapping exceptions → codes HTTP :

| Catégorie | Exception | HTTP | Exposition |
|-----------|-----------|------|-----------|
| Validation FluentValidation | `ValidationException` | 400 | `ValidationProblemDetails` détaillé |
| Validation domaine | `DomainValidationException` | 400 | Message exposé |
| Non trouvé | `UserNotFoundException`, `AccountNotFoundException` | 404 | Message exposé |
| Conflit | `UserEmailAlreadyInUseException` | 409 | Message exposé |
| Précondition métier | `InsufficientFundsException`, `AccountNotActiveException` | 422 | Message exposé |
| Autorisation | `AccountOwnershipException` | 403 | Message exposé |
| Authentification | `UserFailedLoginException` | 401 | Message générique |
| Infrastructure | `ValueEmptyException`, autres | 500 | Message générique masqué |

**Règle de sécurité :** Les erreurs 500 ne retournent jamais les détails techniques (`"An error has occurred. If the issue persists, please contact customer support."`).

#### Versionnage de l'API

Routes avec préfixe `/v1/`. Les routes actuelles sont :
- `POST /api/v1/users/register`
- `POST /api/v1/users/login`
- `POST /api/v1/accounts`
- `GET /api/v1/accounts`
- `POST /api/v1/transactions`

#### Format de sérialisation

JSON avec conversion des enums en string (`JsonStringEnumConverter`) pour la lisibilité et la stabilité du contrat (pas de couplage sur les valeurs numériques des enums).

### Alternatives considérées

| Alternative | Raison du rejet |
|------------|-----------------|
| Exceptions HTTP directement dans les handlers | Couplage entre domaine et HTTP |
| Codes d'erreur applicatifs (Result<T, Error>) | Pattern non adopté pour garder la cohérence avec le style ASP.NET Core |

### Conséquences

**Positives :**
- Contrat d'erreurs uniforme et documenté.
- Sécurité par défaut pour les erreurs d'infrastructure.
- Domaine découplé du transport HTTP.

**Négatives / Risques :**
- Les messages d'erreur exposés pour les codes 4xx peuvent aider des attaquants à énumérer des informations (ex. : distinction entre "email non trouvé" et "mot de passe incorrect" → `UserFailedLoginException` retourne intentionnellement un message générique).

---

## ADR-004 : Conformité, Audit et Stratégie Exactly-Once / Effectively-Once

**Statut :** Accepté

### Contexte

Les exigences réglementaires canadiennes (FINTRAC, OSFI, LPRPDE) imposent une traçabilité complète, des journaux immuables et une garantie d'exactitude des écritures financières. La plateforme doit garantir qu'un virement ne soit débité qu'une seule fois, même en cas de rejeu réseau.

### Décision

#### Stratégie Effectively-Once pour les transactions financières

CanBankX implémente **effectively-once** via :

1. **Idempotency Key** côté client : chaque requête de transaction inclut un UUID unique (`IdempotencyKey`).
2. **Contrainte unique en base** : `UNIQUE INDEX` sur `idempotency_key` dans `transactions`.
3. **Vérification applicative avant insertion** : `GetByIdempotencyKeyAsync()` dans `InitiateTransactionHandler`.
4. **Réponse cohérente** : retour de la transaction existante (HTTP 201, `AlreadyExists=true`) pour les rejeux.

```csharp
// InitiateTransactionHandler.Handle()
var existing = await _transactions.GetByIdempotencyKeyAsync(cmd.IdempotencyKey, ct);
if (existing is not null) return new InitiateTransactionResponse(existing, true);
// ... sinon créer et persister
```

#### Traçabilité et audit par Domain Events

Chaque opération métier significative déclenche un **Domain Event** :

| Opération | Domain Event |
|-----------|-------------|
| Inscription | `UserRegisteredDomainEvent` |
| Confirmation email | `EmailVerifiedDomainEvent` |
| KYC démarré | `KycStartedDomainEvent` |
| KYC approuvé | `KycApprovedDomainEvent` |
| KYC rejeté | `KycRejectedDomainEvent` |
| Ouverture compte | `AccountOpenedDomainEvent` |
| Débit | `AccountDebitedDomainEvent` |
| Crédit | `AccountCreditedDomainEvent` |
| Gel de compte | `AccountFrozenDomainEvent` |
| Fermeture compte | `AccountClosedDomainEvent` |
| Transaction créée | `TransactionCreatedDomainEvent` |
| Transaction complétée | `TransactionCompletedDomainEvent` |
| Transaction échouée | `TransactionFailedDomainEvent` |
| 2FA activé | `TwoFactorEnabledDomainEvent` |

Ces events sont dispatché via MediatR et peuvent être abonnés par des handlers d'audit sans modifier le domaine.

#### Contrôles AML (Anti-Money Laundering)

Les contrôles AML sont préparés architecturalement :
- La table `transactions` contient tous les champs nécessaires (`from_user`, `from_account`, `destination_type`, `amount`, `status`, timestamps).
- Le `TransactionCreatedEventHandler` est le point d'extension prévu pour injecter les contrôles AML (seuils, fréquence, patterns).
- L'architecture domain events permet d'ajouter des handlers AML sans modifier les agrégats.

### Alternatives considérées

| Alternative | Raison du rejet |
|-------------|-----------------|
| Exactly-once avec 2PC | Nécessite un coordinateur de transactions distribué, complexité élevée |

### Conséquences

**Positives :**
- Garantie effectively-once robuste pour les transactions financières.
- Traçabilité complète par domain events.
- Architecture extensible pour les contrôles AML sans modification du domaine.
- Conformité aux exigences d'audit réglementaire.

**Négatives / Risques :**
- Fenêtre de vulnérabilité entre le dispatch de `TransactionCreatedDomainEvent` et l'appel HTTP à Accounts : si ce dernier échoue, la transaction reste `Pending` sans mécanisme de reprise automatique.
- Les domain events ne sont pas persistés de manière durable (outbox absent) : un crash entre `DispatchDomainEventsAsync()` et `SaveChangesAsync()` peut créer des incohérences.
