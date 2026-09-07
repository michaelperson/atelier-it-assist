# .githooks/

Dossier de destination des crochets Git produits pendant l'atelier
d'automatisation. Vide au départ.

Un dossier de crochets versionné n'est pas actif par défaut : Git ne lit que
`.git/hooks`, qui n'est pas suivi. L'activation se fait une fois par poste :

```bash
git config core.hooksPath .githooks
```

C'est la ligne qui rend l'automatisation partageable : le contenu des crochets
est revu en revue de code comme n'importe quel autre fichier du dépôt, et un
nouvel arrivant n'a qu'une commande à exécuter.

À l'arrivée, on doit y trouver :

| Crochet               | Rôle                                                          |
|-----------------------|---------------------------------------------------------------|
| `prepare-commit-msg`  | Propose un message de commit conventionnel à partir du diff    |
| `pre-push`            | Revue automatique du diff avant envoi, bloquante sur signalement critique |

Rappel : un crochet doit être exécutable (`chmod +x`) et doit rester rapide.
Un crochet qui dure plus de quelques secondes se fait contourner par
`--no-verify` dans la semaine.
