using UnityEngine;
using UnityEngine.Playables;

public class cutScenePly : MonoBehaviour
{
    public PlayerAi Player;
    public EnemyAi EnemyAi;
    public CameraFollw CameraFollw;
    public PlayableDirector cutSCene;
    public GameObject uiOff;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Player != null) Player.enabled = false;
        Player.CancelInvoke();
        if (uiOff != null) uiOff.SetActive(false);
        if (EnemyAi != null) EnemyAi.enabled = false;
        if (CameraFollw != null) CameraFollw.enabled = false;
        if (cutSCene != null)
        {
            cutSCene.stopped += onCutsceneEnd;
            cutSCene.Play();
        }
       

    }
      void onCutsceneEnd(PlayableDirector director)
     {
        if (Player != null) Player.enabled = true;
        if (uiOff != null) uiOff.SetActive(true);
        Player.InvokeRepeating(nameof(Player.RandomVoice), 1, 8);
        if (EnemyAi != null) EnemyAi.enabled = true;
        if (CameraFollw != null) CameraFollw.enabled = true;
     }
    private void OnDestroy()
    {
        if (cutSCene != null)
        {
            cutSCene.stopped -= onCutsceneEnd;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
