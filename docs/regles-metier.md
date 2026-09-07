# Règles métier IT-Assist

Document de référence fonctionnel. En cas d'écart entre ce document et le code,
c'est ce document qui fait foi : l'écart est un défaut à corriger.

## 1. Cycle de vie d'un ticket

Statuts : `Nouveau` → `EnCours` → (`EnAttenteDemandeur`) → `Resolu` → `Clos`.

- Un ticket `Resolu` peut repasser à `EnCours` si le demandeur relance.
- Un ticket `Clos` est définitif : aucune transition n'en sort.

## 2. Engagement de prise en charge

Le délai de prise en charge est exprimé en **jours ouvrés** et dépend de la
priorité du ticket :

| Priorité | Prise en charge |
|----------|-----------------|
| Critique | 1 jour ouvré    |
| Haute    | 2 jours ouvrés  |
| Normale  | 5 jours ouvrés  |
| Basse    | 10 jours ouvrés |

Les jours ouvrés excluent le samedi et le dimanche. Les jours fériés belges ne
sont pas encore pris en compte (dette connue et acceptée).

## 3. Escalade

Un ticket non résolu dont le délai de prise en charge est **dépassé** est
escaladé d'un niveau de priorité. Un ticket déjà `Critique` ne s'escalade pas :
il reste `Critique` et remonte au responsable d'astreinte.

Un ticket `Resolu` ou `Clos` ne s'escalade jamais.

## 4. Rattachement des catégories aux équipes

| Catégorie                    | Équipe                  |
|------------------------------|-------------------------|
| Accès et authentification    | Support N1              |
| Poste de travail             | Support N1              |
| Réseau et téléphonie         | Infrastructure          |
| ERP et applications métier   | Applications métier     |
| Données et reporting         | Applications métier     |
| Sécurité                     | Sécurité opérationnelle |

Une catégorie non rattachée est une erreur de configuration, pas un cas nominal :
elle doit remonter en erreur et être journalisée en avertissement.

## 5. Engagements de service et pénalités

Le calcul des délais cibles, des ruptures d'engagement et des pénalités est
porté par `CalculSla`, repris de l'ancien portail interne. Les règles réelles
sont celles du contrat cadre ; **ce document ne les décrit pas** et le code est
aujourd'hui la seule source de vérité sur ce point.
