using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GlobalStatics
{
    const ELanguage DEFAULT_LANGUAGE = ELanguage.Swedish;

    /// <summary>
    /// What localisation/language to use for text
    /// </summary>
    private static ELanguage language;

    /// <summary>
    /// What localisation/language to use for text
    /// </summary>
    public static ELanguage Language
    {
        get
        {
            switch (language)
            {
                case ELanguage.English:
                    return language;

                case ELanguage.Swedish:
                    return language;

                default:
                    return DEFAULT_LANGUAGE;
            }
        }

        set
        {
            language = value;
        }
    }
}

public enum ELanguage
{
    English,
    Swedish
}
