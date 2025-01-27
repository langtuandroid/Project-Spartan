using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TurnSpeed : MonoBehaviour
{
    public ContinuousTurnProviderBase turnProvider;
    public SnapTurnProviderBase snapProvider;

    public XRDirectInteractor[] directInteractors;
    public XRRayInteractor[] rayInteractors;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (PlayerPrefs.HasKey("TurnSpeed"))
            turnProvider.turnSpeed = PlayerPrefs.GetFloat("TurnSpeed");
        else if (!PlayerPrefs.HasKey("TurnSpeed"))
            turnProvider.turnSpeed = 75;
        if (PlayerPrefs.HasKey("SnapTurn"))
        {
            if (PlayerPrefs.GetInt("SnapTurn") >= 1)
            {
                snapProvider.enabled = true;
                turnProvider.enabled = false;
            }
        }
        if (PlayerPrefs.HasKey("ContTurn"))
        {
            if (PlayerPrefs.GetInt("ContTurn") >= 1)
            {
                snapProvider.enabled = false;
                turnProvider.enabled = true;
            }
        }
        if (PlayerPrefs.HasKey("ToggleTurn"))
        {
            if (PlayerPrefs.GetInt("ToggleTurn") >= 1)
            {
                foreach (XRDirectInteractor interactor in directInteractors)
                {
                    interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.Toggle;
                }
                foreach (XRRayInteractor interactor in rayInteractors)
                {
                    interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.Toggle;
                }
            }
        }
        if (PlayerPrefs.HasKey("StateTurn"))
        {
            if (PlayerPrefs.GetInt("StateTurn") >= 1)
            {
                foreach (XRDirectInteractor interactor in directInteractors)
                {
                    interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
                }
                foreach (XRRayInteractor interactor in rayInteractors)
                {
                    interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
                }
            }
        }
    }
}
