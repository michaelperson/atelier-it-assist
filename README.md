# IT-Assist — dépôt d'entraînement

Application de gestion des tickets du service d'assistance interne. Backend
C#/.NET 8, frontend Angular, module historique InterSystems Caché encore en
production.

**Ce dépôt est un support de formation.** L'application est cohérente et
plausible de bout en bout, mais elle n'est pas destinée à être mise en service :
elle existe pour qu'on y travaille avec un assistant d'IA sur du code qui
ressemble à du vrai — avec ses conventions non écrites, sa dette assumée, son
module que plus personne n'ose toucher, et au moins un défaut qui n'a pas été
détecté.

## Arborescence

```
backend/
  src/ItAssist.Domain/       Métier : tickets, routage, engagements de service
  src/ItAssist.Api/          API HTTP, dépôt de données, journalisation
  tests/ItAssist.Tests/      Tests unitaires — couverture partielle assumée
frontend/                    Angular : liste des tickets, tableau de bord
legacy-cache/                Routine ObjectScript en production. Gelée.
data/                        Jeu de tickets synthétique (voir data/README.md)
docs/                        Règles métier — source de vérité fonctionnelle
logs/                        Journal applicatif d'une session réelle
scripts/                     Vide. Outillage produit pendant les ateliers.
.githooks/                   Vide. Crochets Git produits pendant les ateliers.
ateliers/                    Vide. Livrables non-code des ateliers.
```

## Démarrer

```bash
# Backend — API sur http://localhost:5187
dotnet run --project backend/src/ItAssist.Api

# Tests
dotnet test backend/ItAssist.sln

# Frontend — sur http://localhost:4200
cd frontend && npm install && npm start
```

Le backend lit `data/tickets.jsonl`. Le chemin est configurable par
`Tickets:Chemin` dans `appsettings.json`.

```bash
curl http://localhost:5187/sante
curl http://localhost:5187/api/tickets/INC-2026-0241
curl http://localhost:5187/api/tickets/INC-2026-0241/escalade
curl "http://localhost:5187/api/rapports/mensuel?mois=8&annee=2026"
```

## Ce qu'il faut savoir avant d'y toucher

Le dépôt ne contient **pas** de fichier de contexte projet. C'est le premier
livrable de la formation, pas un oubli : `CLAUDE.md.template` en est le gabarit.

Les conventions du projet ne sont écrites nulle part. Elles sont lisibles dans
le code — dans une partie du code. Trois fichiers en particulier sont la
référence de ce que l'équipe considère comme correct :

- `backend/src/ItAssist.Api/Infrastructure/DepotTicketsFichier.cs`
- `backend/src/ItAssist.Domain/Tickets/RoutageService.cs`
- `frontend/src/app/tickets/liste-tickets.component.ts`

D'autres fichiers ne les respectent pas. Faire la différence entre les deux
catégories, sans qu'on vous dise laquelle est laquelle, est le premier exercice.

`docs/regles-metier.md` est la **source de vérité fonctionnelle**. En cas
d'écart entre ce document et le code, c'est le code qui a tort.

## Dette connue et assumée

Ces points sont documentés, décidés, et ne doivent **pas** être « corrigés »
sans discussion :

- Les jours fériés belges ne sont pas pris en compte dans le calcul des jours
  ouvrés (`RoutageService`).
- `CalculSla` est un portage littéral de l'ancien portail interne. Il est appelé
  par la facturation. Aucun test ne le couvre.
- `TicketService` est un service historique qui mélange lecture de fichier,
  règles métier et mise en forme. Il alimente encore le rapport mensuel.
- `legacy-cache/RoutineTicket.mac` est en production et gelé.
- Les dépendances frontend ne sont pas à jour. C'est un choix de calendrier.

Le reste n'est pas de la dette assumée. Le reste est à regarder.

## Périmètre de vérification

Le code C# et le code Angular de ce dépôt ont été **juste compilés**.  

Si `dotnet build` ou `npm run build` ou `ng build` signale une erreur de compilation, ce n'est pas un exercice
caché : c'est un défaut du support, et il faut le signaler.
