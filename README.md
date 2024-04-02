# Test Technique : Visionneuse de casque

### 1. Interface Utilisateur
Ce n'est pas ma spécialité et je ne suis pas pleinement satisfait du résultat. Il y avait probablement une autre manière de faire.
J'ai rajouté une barre de chargement et désactivé l'UI qui contôle la vue en attendant que les requêtes de texture soient faites.

### 2. Création de matériaux
Selon les consignes, la texture metallic/roughness utilise le packing de la spécification glTF 2.0 qui indique que les canaux utilisé sont le R et le G alors que le shader standard utilise les canaux R et A.
Or, en observant la texture, on voit bien que les canaux utilisés sont le G et le B, donc les changements de canaux que j'ai faits sont G -> R et B -> A.

Comme dit précedemment, j'ai ajouté une barre de chargement pendant la création de matériaux.