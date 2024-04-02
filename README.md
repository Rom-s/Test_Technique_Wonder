# Test Technique : Visionneuse de casque

### 1. Interface Utilisateur
Ce n'est pas ma sp�cialit� et je ne suis pas pleinement satisfait du r�sultat. Il y avait probablement une autre mani�re de faire.
J'ai rajout� une barre de chargement et d�sactiv� l'UI qui cont�le la vue en attendant que les requ�tes de texture soient faites.

### 2. Cr�ation de mat�riaux
Selon les consignes, la texture metallic/roughness utilise le packing de la sp�cification glTF 2.0 qui indique que les canaux utilis� sont le R et le G alors que le shader standard utilise les canaux R et A.
Or, en observant la texture, on voit bien que les canaux utilis�s sont le G et le B, donc les changements de canaux que j'ai faits sont G -> R et B -> A.

Comme dit pr�cedemment, j'ai ajout� une barre de chargement pendant la cr�ation de mat�riaux.