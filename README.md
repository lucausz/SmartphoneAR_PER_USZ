# Rapport de TP : Réalité Augmentée sur Smartphone (ARCore)

**Étudiants :** Brice Perrout et Luca Uszynski    

---

## Introduction : Présentation de l'Application "Le Chat Télécommandé"

L'objectif de ce projet était de concevoir une application de Réalité Augmentée fonctionnelle, interactive et scénarisée. Nous avons développé une expérience intitulée **"Le Chat Télécommandé"**. 

Le principe est le suivant : l'utilisateur utilise un marqueur physique (image) pour faire apparaître un chat 3D. Il doit ensuite scanner son environnement réel pour y poser un tapis de jeu virtuel (le *GameBoard*). La mécanique principale repose sur la proximité : si le marqueur du chat est approché suffisamment près du tapis virtuel, l'utilisateur débloque une interface de contrôle (un joystick tactile) lui permettant d'animer et de déplacer librement le chat sur la zone de jeu.

---

## I. Détection du Monde et Placement (Parties 1 & 3)

*Cette partie couvre la configuration AR Foundation et la détection de l'environnement physique.*

### 1. Suivi des surfaces (Plane Tracking)
Afin que l'application comprenne la topologie de la pièce, nous avons intégré un `ARPlaneManager` couplé à un visualiseur (`ARPlanePrefab` semi-transparent). Cela fournit un retour visuel indispensable à l'utilisateur, lui indiquant quelles surfaces horizontales (sol, tables) ont été correctement cartographiées par ARCore.

### 2. Placement interactif du GameBoard
Une fois une surface détectée, l'utilisateur peut tapoter l'écran pour y ancrer le `GameBoard` (la zone de jeu). 
Pour implémenter cette interaction, nous avons strictement utilisé le **New Input System** d'Unity via la classe `EnhancedTouch`. 

* **Le mécanisme :** À chaque `TouchPhase.Began` (contact initial), un rayon (`ARRaycast`) est projeté depuis les coordonnées de l'écran vers l'espace 3D.
* **Le filtrage :** Le rayon est configuré pour ne réagir qu'aux surfaces physiques détectées (`TrackableType.PlaneWithinPolygon`).
* **L'action :** Si le rayon intersecte un plan, nous récupérons les coordonnées exactes de l'impact (`Pose`) pour instancier le plateau à cet endroit, ou le déplacer s'il existe déjà.



---

## II. Détection d'Image et Apparition (Partie 2)

L'ancrage du modèle 3D sur le marqueur physique est géré par l'`ARTrackedImageManager`, lié à notre propre *Reference Image Library*. 

**Architecture Parent/Enfant :**
Pour anticiper les interactions de déplacement (Partie 5), nous avons dû contourner le comportement par défaut d'AR Foundation, qui verrouille le modèle 3D au centre de l'image. Nous avons donc structuré notre Prefab en deux niveaux :
* **Le Parent (`Cat_Parent`) :** Géré à 100% par le moteur de tracking AR. Il reste ancré sur le marqueur physique.
* **L'Enfant (`Cat_3D`) :** Contient le modèle 3D du chat et le composant `Animator`. C'est cet objet que nous manipulons via nos scripts pour le désolidariser visuellement du marqueur sans briser le tracking.



---

## III. Scénario et Interactions Avancées (Parties 4 & 5)

*Cette section détaille le cœur interactif de l'application, liant le monde réel, l'interface utilisateur et les objets virtuels.*

### 1. Condition spatiale d'interaction
Pour que le scénario ait du sens, le marqueur et le plateau de jeu doivent interagir. Dans notre script principal (`JoystickChat`), nous calculons en temps réel la distance séparant le marqueur physique du GameBoard virtuel en utilisant `Vector3.Distance`.
* **Règle métier :** Le joystick de contrôle ne s'active que si la distance est inférieure à 40 cm. Cela oblige l'utilisateur à faire interagir concrètement les différents éléments de l'application dans l'espace réel.

### 2. Interface Utilisateur (UI) et Locomotion
Pour répondre au critère d'extension "UI en Réalité Augmentée", nous avons intégré un *Fixed Joystick* sur le Canvas (en mode d'affichage *Screen Space - Overlay*).

* **Déplacement :** Les entrées 2D du joystick sont converties en vecteur de mouvement 3D (X et Z) appliquées au `Transform.localPosition` de l'enfant (`Cat_3D`).
* **Rotation fluide :** Pour un rendu naturel, le chat s'oriente dans sa direction de marche grâce à la fonction mathématique `Quaternion.Slerp` combinée à `Quaternion.LookRotation`.
* **Animation :** Le script communique en permanence avec l' `Animator` du chat. Dès que la magnitude du joystick dépasse 0.05, le paramètre booléen `isWalking` passe à `true`, déclenchant l'animation de marche. Il repasse à `false` lors de l'arrêt ou si le chat s'éloigne trop du plateau.



---

## IV. Guide d'Utilisation

1. Lancer l'application et autoriser l'accès à la caméra.
2. Scanner l'environnement en effectuant de lents mouvements avec le smartphone (Zone bleue).
3. Tapoter sur une zone de détection (grille) pour instancier le plateau de jeu (Zone rouge).
4. Présenter l'image cible devant la caméra pour faire apparaître le chat (Il faut s'approcher pour que l'image marqueur prenne presque tout l'écran).
5. Le chat apparait alors sur le plateau virtuel.
6. Utiliser le joystick sur l'écran pour diriger et animer le chat sur le tapis (l'animation concerne uniquement des mouvement de tête, les pates ne sont pas animées).
