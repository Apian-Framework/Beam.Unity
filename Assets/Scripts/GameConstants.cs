using UnityEngine;
public enum Heading
{
    // z is North, x is East
    kNorth = 0,
    kEast = 1,
    kSouth = 2,
    kWest = 3,
    kCount = 4
}

public enum TurnDir
{
    kNone = 0, 
    kLeft = 1,
    kRight = 2,
    kUnset = 3 // usually straied as set
}

public enum ScoreEvent
{
    kClaimPlace = 0,
    kHitFriendPlace = 1,
    kHitEnemyPlace = 2,
    kOffMap = 3,
}

public static class GameConstants 
{
    public static readonly int[] eventScores = {
        11, // claimPlace
        -511, // hitfriend
        -1211, // hitEnemy
        0, // offMap (no score issue - you just die)
    };

    // NOTE: coordinates are LEFT-HANDED! A positive heading rotation, for instance, is clockwise from above.
    private static readonly float[] headingDegrees = {
        0f, 90f, 180f, 270f
    };

    public static float HeadingDegrees(Heading h) => headingDegrees[(int)h%4];

    public static readonly Vector3[] unitOffsetForHeading = {
        // Unit velocity for given heading
        new Vector3(0, 0, 1),  // N
        new Vector3(1, 0, 0),  // E
        new Vector3(0, 0, -1),  // S
        new Vector3(-1, 0, 0),  // W         
    };


}