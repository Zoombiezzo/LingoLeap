using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.SpriteService
{
    public interface ISpritesPreset
    {
        string Id { get; }
        IReadOnlyList<SpritePreset> SpritePresets { get; }
    }
}