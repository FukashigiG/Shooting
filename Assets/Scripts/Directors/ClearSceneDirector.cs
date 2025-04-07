using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ClearSceneDirector : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt_ClearTime;
    [SerializeField] TextMeshProUGUI txt_Weapon;
    [SerializeField] TextMeshProUGUI txt_Stage;

    [SerializeField] GameObject fadePanel;

    private void Start()
    {
        switch (StartSceneDirector.stage)
        {
            case StartSceneDirector.stageEnum.stage0:
                txt_Stage.text = "�N���A�X�e�[�W�F�U�E�{�X";
                break;

            case StartSceneDirector.stageEnum.stage1:
                txt_Stage.text = "�N���A�X�e�[�W�F�}�C�N:FS";
                break;

            default:
                txt_Weapon.text = "�N���A�X�e�[�W�F�U�E�{�X";
                break;
        }

        txt_ClearTime.text = "�N���A�^�C���F" + GameDirector.elapsedTime + " �b";

        switch (StartSceneDirector.weapon)
        {
            case StartSceneDirector.weaponEnum.weapon0:
                txt_Weapon.text = "�g�p�E�F�|�� : �V���[�^�[";
                break;

            case StartSceneDirector.weaponEnum.weapon1:
                txt_Weapon.text = "�g�p�E�F�|�� : �T�����C";
                break;
        }

        fadePanel.SetActive(true);
        Image image = fadePanel.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 1f);
        image.DOFade(0f, 1.5f).OnComplete(() => fadePanel.SetActive(false));
    }

    public void GoToStartScene()
    {
        Sceneloader.Instance.LoadScene("StartScene");
    }
}
