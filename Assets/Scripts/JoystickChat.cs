using UnityEngine;

public class JoystickChat : MonoBehaviour
{
    [Header("Les objets à lier")]
    public Transform chatEnfant; 
    // NOUVEAU : La case pour glisser l'animateur
    public Animator animateurChat; 
    
    public float vitesse = 0.3f;
    public float vitesseRotation = 10f;

    private FixedJoystick leJoystick; 
    private GameObject plateau;

    void Start()
    {
        leJoystick = FindObjectOfType<FixedJoystick>();
        plateau = GameObject.FindWithTag("GameBoard");
    }

    void Update()
    {
        // Sécurité : si on n'a pas tout lié, on arrête tout
        if (leJoystick == null || plateau == null || chatEnfant == null || animateurChat == null) return;

        // Condition (Partie 5) : Le chat est proche du plateau
        if (Vector3.Distance(transform.position, plateau.transform.position) < 0.4f) 
        {
            Vector2 input = new Vector2(leJoystick.Horizontal, leJoystick.Vertical);
            
            // Est-ce qu'on touche au joystick ? (vrai ou faux)
            bool estEnMouvement = input.magnitude > 0.05f;

            // NOUVEAU : On envoie l'info au cerveau de l'animation !
            // ATTENTION : Change "isWalking" par le vrai nom de ton paramètre (Étape 2)
            animateurChat.SetBool("State", estEnMouvement);

            if (estEnMouvement) 
            {
                Vector3 directionMouvement = new Vector3(input.x, 0, input.y);
                chatEnfant.localPosition += directionMouvement * vitesse * Time.deltaTime;

                if (directionMouvement != Vector3.zero)
                {
                    Quaternion rotationCible = Quaternion.LookRotation(directionMouvement);
                    chatEnfant.localRotation = Quaternion.Slerp(chatEnfant.localRotation, rotationCible, Time.deltaTime * vitesseRotation);
                }
            }
        }
        else
        {
            // Si le chat est trop loin du plateau, on le force à s'arrêter (animation d'attente)
            animateurChat.SetBool("State", false);
        }
    }
}