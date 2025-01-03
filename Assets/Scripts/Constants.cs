using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    //Game Related
    public const float MULTIPLICATION_FIELD_MULTIPLIER = 0.5f;
    public const int STANDARD_PLANT_POINTS = 20;
    
    //UI Related
    public const float HOVERED_ALPHA_VALUE = .3f;
    public const float DEATH_SATURATION_VALUE = 0f;
    public static readonly Color ECO_POINTS_COLOR = new Color(0, 152/256f, 18/256f);
    public static readonly Color NEUTRAL_GRAY_COLOR = new Color(116/256f, 116/256f, 116/256f);
    public static readonly Color MULTIPLICATION_COLOR = new Color(220/256f, 20/256f, 20/256f);
    public static readonly Color MULTIPLICATION_FIELD_COLOR = new Color(255/256f, 160/256f, 0/256f);
    public const float UI_POINT_SPEED = .5f;
    public const float UI_POINT_DISAPPEAR_SPEED = .25f;
    public const float UI_POINT_WAIT_INTERVAL = .25f;
    public const int AMOUNT_OF_PICK_CARDS = 3;
    public const float UI_FADE_SPEED = .3f;
    public const float UI_FAST_FADE_SPEED = .1f;
    
    //GameManager Stuff
    public const float CLICK_COOLDOWN = .1f;
}
