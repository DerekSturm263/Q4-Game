using System.Collections.Generic;

public class AbilityTutorial
{
    public byte ability;

    public string name;
    public string description;

    public static Dictionary<byte, AbilityTutorial> abilities = new Dictionary<byte, AbilityTutorial>();

    public AbilityTutorial(byte ability, string name, string description)
    {
        this.ability = ability;

        this.name = name;
        this.description = description;

        abilities.Add(ability, this);
    }
}
