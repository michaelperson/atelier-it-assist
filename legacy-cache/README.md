# Le module historique InterSystems Caché

`RoutineTicket.mac` est une routine ObjectScript qui porte encore la création,
les transitions de statut et l'export des tickets vers le portail .NET. Elle est
appelée par un batch de nuit et par un écran de saisie toujours en service.

Aucune migration n'est prévue à court terme : le périmètre fonctionnel est
stable, la routine tourne, et personne dans l'équipe ne sait la faire évoluer
avec confiance.

## Pourquoi ce fichier est dans le dépôt d'atelier

Parce que c'est le cas où l'assistant d'IA est le moins fiable, et où il est le
plus utile de savoir pourquoi.

ObjectScript est très peu représenté dans les données d'entraînement des
modèles, sa syntaxe est extrêmement condensée (abréviations à une lettre,
opérateurs de manipulation de chaînes propriétaires, structures de données
hiérarchiques globales), et elle ressemble superficiellement à d'autres langages
sans en partager la sémantique. Le résultat est prévisible : un assistant
interrogé sans contexte produira du code plausible et faux — fonctions
inventées, confusion entre `$PIECE` et `$EXTRACT`, indexation de globale traitée
comme un tableau classique, blocs `do` mal indentés (l'indentation par points
est significative).

C'est exactement le matériau dont on a besoin :

1. **Voir l'hallucination sur un cas réel**, pas sur un exemple de démonstration.
2. **Constater ce que le contexte change** : la même question posée avec la
   structure des globales et deux exemples de la maison donne un résultat
   utilisable.
3. **Identifier la limite qui reste** : sur cette pile, l'assistant est un
   accélérateur de compréhension (expliquer, documenter, écrire des tests de
   caractérisation) bien avant d'être un producteur de code.

## Points de repère pour la lecture

| Élément                      | Signification                                                      |
|------------------------------|--------------------------------------------------------------------|
| `s` / `n` / `d` / `q` / `f`  | `set`, `new`, `do`, `quit`, `for`                                  |
| `^TICKET(id)`                | globale : structure persistante hiérarchique                       |
| `$p(x,"^",4)`                | `$PIECE` : 4e champ de `x` délimité par `^`                        |
| `$o(...)`                    | `$ORDER` : parcours ordonné des indices d'une globale              |
| `$H`                         | date et heure internes (jours depuis 1840-12-31, secondes du jour) |
| `$i(^TICKET)`                | `$INCREMENT` : compteur atomique                                   |
| `. ` en début de ligne       | niveau d'imbrication d'un bloc `do` — **significatif**             |
| `$$DELAIOUV(id)`             | appel d'une fonction extrinsèque de la routine                     |

## Ce que la routine fait vraiment

- `CREER` : alloue un identifiant, écrit la ligne délimitée, indexe par service.
- `STATUT` : applique les transitions ; `4` est un état terminal, `3` ne peut
  aller que vers `1` ou `4`.
- `DELAIOUV` : compte les jours ouvrés entre ouverture et résolution.
- `PARSRV` / `RECALCIDX` : lecture par index, et reconstruction de cet index.
- `EXPORT` : export délimité par `|` consommé par le portail .NET.

Ces cinq phrases n'existaient pas avant l'atelier. C'est leur production, et
leur vérification, qui est l'exercice.
