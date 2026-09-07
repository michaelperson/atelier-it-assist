# Jeu de données de tickets

`tickets.jsonl` : 252 tickets, un objet JSON par ligne.

```json
{"id":"INC-2026-0001","titre":"…","categorie":"…","priorite":"Normale",
 "statut":"Résolu","demandeur":"…","service":"…","site":"…",
 "ouvert_le":"2026-03-12","description":"…","resolution":"…"}
```

| Champ        | Valeurs                                                                  |
|--------------|--------------------------------------------------------------------------|
| `priorite`   | `Basse`, `Normale`, `Haute`, `Critique`                                   |
| `statut`     | `Nouveau`, `En cours`, `En attente demandeur`, `Résolu`, `Fermé`          |
| `categorie`  | six catégories, voir `docs/regles-metier.md`                              |
| `ouvert_le`  | date ISO, tickets de janvier à septembre 2026                             |
| `resolution` | vide pour les tickets encore ouverts                                      |

## Origine et statut juridique

**Ce jeu de données est entièrement synthétique.** Il a été fabriqué pour la
formation : aucune donnée provenant d'un outil de ticketing réel n'y figure.
Les noms, adresses, numéros de téléphone, matricules, numéros de compte et noms
de postes sont inventés.

Il est en revanche fabriqué **pour ressembler à un export réel**, y compris dans
ce qu'un export réel contient de gênant. Les motifs présents, et leurs volumes :

| Motif                          | Occurrences | Exemple                          |
|--------------------------------|-------------|----------------------------------|
| Nom du demandeur (champ)       | 252         | `"demandeur": "Isabelle Declercq"` |
| Nom de personne dans le texte  | fréquent    | « Isabelle Declercq branche son portable… » |
| Nom d'intervenant (résolutions)| 24          | « Escaladé à Bruno Segers (équipe Infrastructure) » |
| Adresse de courriel            | 56          | `fabrice.lambert@exemple-interne.be` |
| Téléphone, format national     | 34          | `0475 62 18 04`, `081/22 41 07`  |
| Téléphone, format international| 31          | `+32 478 42 31 20`               |
| Nom de poste de travail        | 38          | `PC-NAM-1210`                    |
| Numéro de compte (format belge)| 18          | `BE58 1181 4963 9481`            |
| Matricule                      | 9           | `matricule MAT93397`             |

Deux remarques sur ce tableau, qui font partie de l'exercice :

- **Les deux formats de téléphone.** L'export en contient deux, national et
  international. Une liste de motifs qui n'en prévoit qu'un laisse passer la
  moitié des numéros — et le recomptage automatique affichera zéro, parce qu'il
  compte avec le même motif incomplet.
- **Les noms d'intervenants.** Les techniciens et prestataires cités dans les
  résolutions ne sont demandeurs d'aucun ticket. Une liste nominative construite
  depuis le champ `demandeur` ne les couvre donc pas.

C'est volontaire. Un atelier d'anonymisation qui s'exerce sur un jeu propre
n'apprend rien — et un anonymiseur qui ne traite que les adresses de courriel
donne une fausse assurance, ce qui est pire que pas d'anonymiseur du tout.

Le comptage est reproductible :

```bash
python3 - <<'PY'
import json, re
d = [json.loads(l) for l in open("data/tickets.jsonl", encoding="utf-8") if l.strip()]
txt = " ".join(t["description"] + " " + t["resolution"] for t in d)
for nom, rx in [("courriels", r"[\w.\-]+@[\w.\-]+\.[a-z]{2,}"),
                ("telephones", r"\b0\d{1,3}[/ ]\d{2,3}( \d{2}){2,3}\b"),
                ("IBAN", r"BE\d{2}(?: \d{4}){3}"),
                ("postes", r"\b[A-Z]{3}-\d{4}\b")]:
    print(nom, len(re.findall(rx, txt)))
PY
```

Deux avertissements qui font partie de l'exercice :

1. **Le nom de personne n'a pas de motif.** Les cinq autres motifs se
   détectent par expression régulière ; celui-là non. C'est la difficulté
   réelle de l'anonymisation, et elle ne se résout pas par une regex de plus.
2. **Le champ `resolution` est le plus sensible.** C'est là que se trouvent les
   noms de systèmes internes, les contournements, et les indices d'incidents de
   sécurité. C'est aussi le champ le plus utile à une recherche documentaire.
   L'arbitrage entre les deux est une décision, pas un réglage.

## Ce que le jeu ne contient pas

Pas de pièce jointe, pas de fil de conversation, pas d'horodatage de résolution,
pas de rattachement à un contrat de service. Ces manques sont eux-mêmes un sujet :
la première question d'une architecture de recherche documentaire est de savoir
ce qui manque dans la source.
