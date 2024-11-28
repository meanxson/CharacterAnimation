using UnityEngine;
using Zenject;

namespace Client
{
    public class GameplayInstaller : MonoInstaller<GameplayInstaller>
    {
        [SerializeField] private PlayerBehaviour _player;

        public override void InstallBindings()
        {
            Container.Bind<PlayerBehaviour>().FromInstance(_player).AsSingle().NonLazy();
        }
    }
}