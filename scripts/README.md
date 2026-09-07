# scripts/

Dossier de destination des outils produits pendant les ateliers.

Rien n'est fourni ici au départ : c'est le contenu que les participants
écrivent, avec leur assistant, qui remplit ce dossier.

À l'arrivée, on doit y trouver :

| Fichier attendu       | Atelier | Rôle                                               |
|-----------------------|---------|----------------------------------------------------|
| `anonymiser.py`       | Gouvernance | Retire les données personnelles d'un export de tickets |
| `indexer.py`          | RAG     | Construit l'index de recherche sur les tickets      |
| `chercher.py`         | RAG     | Renvoie les tickets les plus proches d'une question |
| `tests/`              | plusieurs | Tests des outils ci-dessus                        |

Les scripts d'atelier sont en Python : c'est le seul interpréteur disponible sur
tous les postes sans installation, et l'outillage n'est pas le sujet de la
formation — le code métier, lui, reste en C# et en TypeScript.
