public static class Abilities
{
    public static AbilityTutorial wallClimbing = null;
    public static AbilityTutorial nightVision = null;
    public static AbilityTutorial underwaterBreath = null;

    public static bool Exists()
    {
        return wallClimbing != null;
    }

    public static void Initialize()
    {
        wallClimbing = new AbilityTutorial(0b_0000_0001, "Climbing Walls", "You can now climb walls!\n\nPressing UP against a wooden wall will allow you to climb it.\nPressing JUMP will make you jump off of the wall.");
        nightVision = new AbilityTutorial(0b_0000_0010, "Night Vision", "You now have night vision!\n\nYour night vision will automatically activate at night, and when in extremely dark areas, it allows you to see more clearly.");
        underwaterBreath = new AbilityTutorial(0b_0000_0100, "Underwater Breath", "You can now last longer underwater!\n\nYou can now be underwater for double the amount of time without drowning. Try exploring new areas on the map!");
    }
}
