using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public HeroBlankList heroBlankList;
    public CreateHeroWindow createHeroWindow;
    public GameObject game;

    private void Start()
    {
        foreach(Hero hero in Helper.instance.LoadHeroes())
        {
            heroBlankList.AddHero(hero);
        }
    }

    public void StartGame()
    {
        if (heroBlankList.selected == null)
        {
            createHeroWindow.Show();
            return;
        }

        game.SetActive(true);
        game.GetComponent<GameController>().StartStory(Helper.instance.LoadHero(heroBlankList.selected.heroName.text));
        gameObject.SetActive(false);
    }
}
