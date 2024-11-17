using System.Collections.Generic;
using UnityEngine;

namespace Project.Application.Utility
{
    public static class AudioSettings
    {
        public static int RandomIndexAudioClip(in List<AudioClip> list, int previousIndex = -1)
        {
            int index = Random.Range(0, list.Count);
            if (list.Count > 1 && index == previousIndex) return RandomIndexAudioClip(in list, previousIndex);
            return index;
        }
    }
}