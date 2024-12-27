using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants 
{
    #region Slider

    public const float SLIDER_MAX_VALUE = 1.0f;
    public const float SLIDER_MIN_VALUE = 0.0f;
    public const float SLIDER_NEAR_ZERO = 0.05f;

    #endregion

    #region Range

    public const float NEAR_ZERO_THRESHOLD_2 = 0.1f;
    public const float NEAR_ZERO_THRESHOLD = 0.01f;
    public const float JOYSTICK_THRESHOLD_MIN = 0.1f; //Ngưỡng tối thiểu mà joystick đc coi là moving
    public const float MAX_WIDTH_OF_MAP = 6.5f;
    public const float MIN_WIDTH_OF_MAP = -6.5f;
    public const float CAT_STOPPING_DISTANCE = 0.7f;
    public const int DEFAULT_VALUE_ZERO_INT = 0;

    #endregion

    #region Tags

    public const string WAVE_TAG = "Wave";
    public const string CAT_TAG = "Cat";
    public const string PLAYER_NAME = "Player";

    #endregion

    #region Animation Parameters

    public const string STATE = "state";

    #endregion

    #region Numbers

    public const float REVERSE_AXIS_FACTOR = -1.0f;
    public const float A_SECOND = 1.0f;
    public const float BOUNTY_EACH_CAT = 50.0f;
    public const float BOUNTY_EACH_METTER = 0.25f;
    public const float TRIPLE_REWARD = 3.0f;
    public const float DEFAULT_VALUE_ZERO = 0.0f;
    public const float DEFAULT_VALUE_ONE = 1.0f;
    public const float DECREASE_VELO_EACH_CAT = 1.1f;

    #endregion

    #region File Path

    public const string PATH_PREFAB_TO_LOAD = "PrefabToLoad";

    #endregion
}
