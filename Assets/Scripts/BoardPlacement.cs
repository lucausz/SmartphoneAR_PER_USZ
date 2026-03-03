using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch; // Le nouveau système demandé
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[RequireComponent(typeof(ARRaycastManager))]
public class BoardPlacement : MonoBehaviour
{
    public GameObject gameBoardPrefab; // Glisse ton Prefab GameBoard ici dans l'inspecteur
   
    private GameObject spawnedBoard;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // On active la détection tactile avancée quand le script démarre
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        // On vérifie s'il y a au moins un doigt sur l'écran
        if (Touch.activeTouches.Count > 0)
        {
            // On prend le premier doigt
            Touch touch = Touch.activeTouches[0];

            // On s'assure que c'est le moment précis où le doigt touche l'écran (Began)
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                // On lance un rayon depuis l'écran vers les plans détectés (PlaneWithinPolygon)
                if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    // On récupère les coordonnées exactes (Pose) de l'impact
                    Pose hitPose = hits[0].pose;

                    // Si le plateau n'existe pas encore, on le crée
                    if (spawnedBoard == null)
                    {
                        spawnedBoard = Instantiate(gameBoardPrefab, hitPose.position, hitPose.rotation);
                    }
                    // S'il existe déjà, on le déplace simplement au nouveau point
                    else
                    {
                        spawnedBoard.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                    }
                }
            }
        }
    }
}