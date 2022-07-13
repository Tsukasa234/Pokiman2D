using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    public PokemonBase _Base { get => _base; set => _base = value; }
    [SerializeField] private int level;
    public int Level { get => level; set => level = value; }
    public bool isPlayer;

    public Pokemon pokemon { get; set; }

    private Image pokemonImage;

    private Vector3 initialPosition;
    private Color initialColor;
    [SerializeField] private float playStartAnimationTime = 1.0f, playStartAnimationAttack = 0.3f, 
            hitTimeAnimation = 0.15f, playStartAnimationFaint = 1.0f;

    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition;
        initialColor = pokemonImage.color;
    }

    /// <summary>
    ///Metodo para configurar al pokemon que estara en la batalla dependiendo de si es enemigo o el player
    ///</summary>
    public void SetupPokemon(Pokemon pPokemon)
    {
        pokemon = pPokemon;

        pokemonImage.sprite = (isPlayer ? pokemon.BasePokemon.BackSprite
            : pokemon.BasePokemon.FrontSprite);
        pokemonImage.color = initialColor;
        PlayStartAnimation();
    }

    public void PlayStartAnimation()
    {
        pokemonImage.transform.localPosition =
            new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * 400, initialPosition.y, initialPosition.z);

        pokemonImage.transform.DOLocalMoveX(initialPosition.x, playStartAnimationTime);
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x + (isPlayer ? 1 : -1) * 80, playStartAnimationAttack));
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x, playStartAnimationAttack));
    }

    public void PlayReceiveDamageAnimation()
    {
        var seq = DOTween.Sequence();
        for (var i = 0; i < 2; i++)
        {
            seq.Append(pokemonImage.DOColor(Color.red, hitTimeAnimation));
            seq.Append(pokemonImage.DOColor(initialColor, hitTimeAnimation));
        }
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveY(initialPosition.y - 200, playStartAnimationFaint));
        seq.Join(pokemonImage.DOFade(0, playStartAnimationFaint));
    }
}
