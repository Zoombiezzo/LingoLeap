using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpriteService
{
    public interface ISpritePreset
    {
        string Id { get; }
        Sprite Sprite { get; }
    }
}