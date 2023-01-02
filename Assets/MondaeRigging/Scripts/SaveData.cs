using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SaveData : MonoBehaviour
{
    public int SkillPoints;
    public TopReactsLeaderboard leaderboard;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        object savedCints;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTS, out savedCints))
        {
            SkillPoints = (int)savedCints;
        }
        else
            SkillPoints = 30;

        ExitGames.Client.Photon.Hashtable cints = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, SkillPoints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cints);
        Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Obsolete]
    public void UpdateSkills(int skills)
    {
        SkillPoints += skills;
        ExitGames.Client.Photon.Hashtable cints = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, SkillPoints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cints);
        Save();
    }

    [System.Obsolete]
    public void Save()
    {
        ES3AutoSaveMgr.Current.Save();
        StartCoroutine(SubmitScore());
    }

    [System.Obsolete]
    IEnumerator SubmitScore()
    {
        yield return new WaitForSeconds(3f);
        yield return leaderboard.SubmitScoreRoutine(SkillPoints);
    }
}
