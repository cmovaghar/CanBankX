# CanBankX

Plateforme bancaire commerciale canadienne.

## Stack technique

- **Backend** : C# / .NET 8, architecture microservices (DDD + CQRS)
- **Base de données** : PostgreSQL 16 (une instance par service)
- **Gateway** : nginx 1.27
- **Frontend** : Blazor WASM (Static)
- **Conteneurisation** : Docker / Docker Compose

## Démarrage rapide

```bash
# 1. Copier et configurer les variables d'environnement
cp env.example .env

# 2. Lancer l'application
docker compose up -d

# 3. Vérifier que tout est healthy
docker compose ps
```

L'application est accessible sur **http://localhost:8080**.

> Voir le [Runbook](docs/runbook.md) pour la configuration détaillée des variables d'environnement et les comptes de test.

## Services

| Service | Responsabilité | Port interne |
|---------|---------------|-------------|
| `canbankx-users` | Identité, KYC, authentification JWT | 8081 |
| `canbankx-accounts` | Comptes bancaires (chèques, épargne) | 8081 |
| `canbankx-transactions` | Paiements, virements, idempotence | 8081 |
| `canbankx-gateway` | nginx reverse proxy + Blazor WASM | 8080 (public) |

## Documentation

| Document | Description |
|----------|-------------|
| [Arc42](docs/arc42.md) | Documentation d'architecture |
| [Vue 4+1](docs/4+1.md) | Vues logique, processus, développement, déploiement et scénarios |
| [ADRs](docs/ADRs.md) | Décisions architecturales |
| [Critères d'acceptation](docs/use-cases.md) | Cas d'utilisations |
| [Runbook](docs/runbook.md) | Installation, configuration et utilisation |
