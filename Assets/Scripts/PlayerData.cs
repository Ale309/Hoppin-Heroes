using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    private static int savedCollectibles = 0;

    public static void SaveCollectibles(int collectibles)
    {
        savedCollectibles = collectibles;
    }

    public static int LoadCollectibles()
    {
        return savedCollectibles;
    }
}