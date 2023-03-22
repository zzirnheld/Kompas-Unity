using KompasCore.GameCore;
using UnityEngine;
using UnityEngine.UI;

public class DeckAcceptedUIController : MonoBehaviour
{
    public Image friendlyAvatarImage;
    public Image enemyAvatarImage;

    public void ShowFriendlyAvatar(string avatarFileName)
    {
        var image = CardRepository.LoadSprite(avatarFileName);
        friendlyAvatarImage.sprite = image;
    }

    public void ShowEnemyAvatar(string avatarFileName)
    {
        var image = CardRepository.LoadSprite(avatarFileName);
        enemyAvatarImage.sprite = image;
        enemyAvatarImage.color = Color.white;
    }
}
