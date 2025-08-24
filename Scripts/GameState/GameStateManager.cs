using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
     IGameState Next { get; set; }
     void Init();
     IGameState Enter();
     IGameState RunNextState();
     void Exit();
}

namespace PlayState
{
    public class Loading : IGameState
    {
        public IGameState Next { get; set; }

        public IGameState Enter()
        {
            return this;
        }

        public void Exit()
        {

        }

        public void Init()
        {

        }

        public IGameState RunNextState()
        {
            return Next.Enter();
        }
    }
    public class Spawning : IGameState
    {
        public IGameState Next { get; set; }

        public IGameState Enter()
        {

            UnitManager.Instance.CreatePlayer();
            CameraController.instance.AssignTarget(UnitManager.Instance.player.transform);

            return this;
        }

        public void Exit()
        {

        }

        public void Init()
        {

        }

        public IGameState RunNextState()
        {
            return this;
        }
    }

    public class Playing : IGameState
    {
        public IGameState Next { get; set; }
        private Spawner spawner;
        public IGameState Enter()
        {
            spawner = UnitManager.Instance.spanwer;
            return this;
        }

        public void Exit()
        {

        }

        public void Init()
        {

        }

        public IGameState RunNextState()
        {
            return this;
        }
    }
    public class Ending : IGameState
    {
        public IGameState Next
        {
            get;
            set;
        }

        public IGameState Enter()
        {
            return this;
        }

        public void Exit()
        {

        }

        public void Init()
        {

        }

        public IGameState RunNextState()
        {
            return this;
        }
    }
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public PlayState.Loading loading;
    public PlayState.Spawning spawning;
    public PlayState.Playing playing;
    public PlayState.Ending ending;

    public IGameState current;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        loading = new PlayState.Loading();
        spawning = new PlayState.Spawning();
        playing = new PlayState.Playing();
        ending = new PlayState.Ending();

        loading.Next = spawning;
        spawning.Next = playing;
        playing.Next = ending;
        ending.Next = loading;

        current = loading;
        current.Enter();
    }

    // Update is called once per frame
    void Update()
    {
       current = current.RunNextState();
    }
}
