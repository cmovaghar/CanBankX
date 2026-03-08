# Runbook — CanBankX

## Prérequis

Installer **Docker Desktop** (inclut Docker Compose) :

- **Windows / macOS** : https://www.docker.com/products/docker-desktop
- **Linux** : https://docs.docker.com/engine/install/

Vérifier l'installation :

```bash
docker --version
docker compose version
```

---

## Configuration des variables d'environnement

Créer le fichier `.env` en se basant sur le fichier `env.example`.

Le fichier `.env` contient les variables suivantes à personnaliser selon votre environnement :

```env
# Base de données — Users
USERS_DB_NAME=users_db
USERS_DB_USER=users
USERS_DB_PASS=<mot_de_passe>

# Base de données — Accounts
ACCOUNTS_DB_NAME=accounts_db
ACCOUNTS_DB_USER=accounts
ACCOUNTS_DB_PASS=<mot_de_passe>

# Base de données — Transactions
TRANSACTIONS_DB_NAME=transactions_db
TRANSACTIONS_DB_USER=transactions
TRANSACTIONS_DB_PASS=<mot_de_passe>

# JWT — secret partagé entre les services (minimum 32 caractères)
JWT_SECRET=<secret>

# Chiffrement AES-256 du NAS (SIN)
KEY=<clé_base64_32_bytes>
IV=<iv_base64_16_bytes>

# Authentification inter-services
INTERNAL_HEADER=X-Internal-Secret
INTERNAL_SECRET=<secret_interne>
```

> **Note :** Ne jamais committer le fichier `.env` dans le dépôt Git. Il est déjà ignoré dans `.gitignore`.

---

## Démarrage

Depuis la racine du projet :

```bash
docker compose up -d
```

Docker va construire les images et démarrer les conteneurs dans l'ordre suivant :
1. Les bases de données PostgreSQL (users, accounts, transactions)
2. Les services applicatifs (users, accounts, transactions) — après que leurs bases de données soient prêtes
3. La gateway nginx + frontend Blazor — après que les 3 services soient healthy

Vérifier que tous les conteneurs sont démarrés :

```bash
docker compose ps
```

Tous les services doivent afficher le statut `healthy` ou `running`.

L'application est accessible à l'adresse : **http://localhost:8080**

---

## État du frontend

Le frontend Blazor WASM est **partiellement fonctionnel** :

| Page | État |
|------|------|
| Inscription (`/register`) | Fonctionnelle |
| Connexion (`/login`) | Fonctionnelle |
| Toutes les autres pages | Non fonctionnelles |

Pour tester les fonctionnalités au-delà du login et de l'inscription (gestion des comptes, transactions, etc.), utiliser la **collection Postman** fournie avec le projet.

---

## Utilisation avec Postman

Importer la collection Postman incluse dans le projet (/postman/CanBankX.postman_collection).

L'ordre recommandé pour tester un flux complet :

1. `POST /api/users/register` — créer un compte utilisateur
2. `POST /api/users/login` — récupérer le token JWT
3. Ajouter le token dans le header `Authorization: Bearer <token>`
4. `POST /api/accounts` — ouvrir un compte bancaire
5. `GET /api/accounts` — consulter les comptes
6. `POST /api/transactions` — initier un virement

> **Compte de test pré-créé Admin** : email `admin@admin.com`, mot de passe `admin` (statut "KYC approuvé", prêt à l'usage).

> **Compte de test pré-créé Test** : email `test@test.com`, mot de passe `test` (statut "Courriel non vérifié", permet de tester la validation du JWT dans les endpoints nécéssitant un utilisateur vérifié).

---

## Arrêt

```bash
docker compose down
```

Pour également supprimer les volumes (supprime toutes les données) :

```bash
docker compose down -v
```