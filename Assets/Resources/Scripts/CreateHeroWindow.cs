using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateHeroWindow : GameWindow
{
    [Header("Window components")]
    public Text title;
    public InputField textfield;
    public Button btnCreate, btnCancel;
    public Image fade;
    public Text Counter;
    public HeroBlankList heroBlankList;

    private Text btnCreateText;

    private void Start()
    {
        Hide();

        btnCreateText = btnCreate.GetComponent<Text>();

        textfield.onValueChanged.AddListener(UpdateBtnCreateText);
        textfield.onValueChanged.AddListener(UpdateCounter);

        btnCreate.onClick.AddListener(CreateHero);
        btnCreate.onClick.AddListener(Hide);
        btnCancel.onClick.AddListener(Hide);
    }

    public override void Show()
    {
        textfield.text = "";
        UpdateBtnCreateText("");
        UpdateCounter("");
        base.Show();
        fade.enabled = true;
    }

    public override void Hide()
    {
        base.Hide();
        fade.enabled = false;
    }

    private void UpdateBtnCreateText(string name)
    {
        bool canCreate = true;
        if (name.Length == 0) canCreate = false;
        if (Helper.instance.ContainsHero(name)) canCreate = false;

        btnCreate.interactable = canCreate;

        btnCreateText.text = string.Format("Create {0}", name);
    }

    private void UpdateCounter(string name)
    {
        Counter.text = (textfield.characterLimit - name.Length).ToString();
    }

    private void CreateHero()
    {
        Hero hero = Helper.instance.CreateHero(textfield.text);

        heroBlankList.GetComponent<HeroBlankList>().AddHero(hero);
    }
}
