using UnityEngine;

public class MarkerInteraction : MonoBehaviour
{
    [Header("Paramètres d'interaction")]
    public float interactionDistance = 0.3f; // Distance en mètres (30 cm)
    public Color highlightColor = Color.green; // Couleur quand le marqueur est proche

    private GameObject gameBoard;
    private Material boardMaterial;
    private Color originalColor;
    private bool isInteracting = false;

    void Update()
    {
        // 1. Chercher le plateau s'il n'a pas encore été trouvé
        if (gameBoard == null)
        {
            gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
           
            // Si on vient de le trouver, on sauvegarde sa couleur d'origine
            if (gameBoard != null)
            {
                boardMaterial = gameBoard.GetComponent<MeshRenderer>().material;
                originalColor = boardMaterial.color;
            }
        }

        // 2. Si le plateau existe dans la scène, on calcule la distance
        if (gameBoard != null)
        {
            // Calcul mathématique de la distance entre le marqueur (this) et le plateau
            float distance = Vector3.Distance(transform.position, gameBoard.transform.position);

            // 3. Déclenchement de l'interaction spatiale
            if (distance <= interactionDistance)
            {
                if (!isInteracting)
                {
                    // Le marqueur entre dans la zone !
                    boardMaterial.color = highlightColor;
                    isInteracting = true;
                    Debug.Log("Interaction déclenchée !");
                   
                    // BONUS : Tu pourrais ajouter un effet sonore ici
                    // ou faire apparaître un objet sur le plateau
                }
            }
            else
            {
                if (isInteracting)
                {
                    // Le marqueur sort de la zone
                    boardMaterial.color = originalColor;
                    isInteracting = false;
                }
            }
        }
    }
}