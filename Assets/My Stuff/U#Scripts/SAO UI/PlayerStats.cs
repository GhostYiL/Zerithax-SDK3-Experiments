
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using zerithax.HandSwipeUI;

namespace zerithax.HandSwipeUI
{
    public class PlayerStats : UdonSharpBehaviour
    {
        [Header("Stats' Text")]
        [SerializeField] private TextMeshPro playerNameText;
        [SerializeField] private TextMeshPro strengthText;
        [SerializeField] private TextMeshPro agilityText;

        [Header("Base Stats")]
        [SerializeField] private int health;
        [SerializeField] private int speed;
        [SerializeField] private int strength;
        [SerializeField] private int agility;

        void Start()
        {
            playerNameText.text = Networking.LocalPlayer.displayName;
        }
    }
}