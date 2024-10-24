using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game parameters")]
    [SerializeField] private int _wheatOutput = 0;
    [SerializeField] private int _wheatCount = 0;
    [SerializeField] private int _wheatForCountWarrior = 0;
    [SerializeField] private int _peasantCount = 0;
    [SerializeField] private int _peasantCost = 0;
    [SerializeField] private int _warriorsCount = 0;
    [SerializeField] private int _warriorsCost = 0;

    [Header("Windows")]
    [SerializeField] private GameObject[] _screenGroups;
    [SerializeField] private GameObject[] _allScreens;
    [SerializeField] private GameObject _startScreenGroup;
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _gameResult;

    [Header("Buttons")]
    [SerializeField] private Button _peasantButton;
    [SerializeField] private Button _warriorButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _resoucersText;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _raidersText;
    [SerializeField] private TextMeshProUGUI _gameResultTextResult;
    [SerializeField] private TextMeshProUGUI _gameResultText;

    [Header("Timers")]
    [SerializeField] private Timer _harvestTimer;
    [SerializeField] private Timer _eatingTimer;
    [SerializeField] private Timer _raidTimer;
    [SerializeField] private Timer _peasantTimer;
    [SerializeField] private Timer _warriorTimer;

    [Header("Audios")]
    [SerializeField] private AudioListener _audioListener;
    [SerializeField] private AudioSource _gameMenuAudio;
    [SerializeField] private AudioSource _timerEatingAudio;
    [SerializeField] private AudioSource _harvestAudio;
    [SerializeField] private AudioSource _raidAudio;

    [Header("Images")]
    [SerializeField] private RawImage[] _soundImage;
    [SerializeField] private Texture _soundImageOn;
    [SerializeField] private Texture _soundImageOff;

    private Timer[] _allTimers;
    private GameObject _currentScreen = null;
    private GameObject _currentScreenGrup = null;
    private int _currentWheatCount;
    private int _currentPeasantCount;
    private int _currentWarriorsCount;
    private int _wave = 0;
    private int _raidersCount = 0;
    private bool _gamePlay = false;
    private bool _isPaused = false;
    private bool _sound = true;
    
    private int _hiredWarriors = 0;     
    private int _deadRaiders = 0;   
    private int _harvestedWheat = 0;
    private int _maxWarriors = 0;

    void Start()
    {
        _allTimers = new Timer[5] { _harvestTimer, _eatingTimer, _raidTimer, _peasantTimer, _warriorTimer };
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gamePlay)
        {
            CheckGameOverConditions();

            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause(!_isPaused);
            
            UpdateResourceCount();
            CheckEnoughMoney();
            UpdateWave();
        }
    }

    public void StateMachine (GameObject targetScreen) // Переключение экранов
    {
        _currentScreen?.SetActive(false);
        targetScreen.SetActive(true);
        _currentScreen = targetScreen;   
    }

    public void HandleGameState(GameObject targetScreenGrup) // Переключение между групп экранов 
    {
        _currentScreenGrup?.SetActive(false);
        targetScreenGrup.SetActive(true);
        _currentScreenGrup = targetScreenGrup;
    }

    public void TogglePause(bool pause)
    {
        _gameMenuAudio.Play();
        StateMachine(pause ? _pauseMenu : _gameUI);
        Time.timeScale = pause ? 0f : 1f;
        _isPaused = pause;
    }

    public void InitGame()
    {
        _isPaused = false;
        _gamePlay = false;
        Time.timeScale = 1f;

        foreach (GameObject screen in _allScreens)
            screen.SetActive(false);
        foreach (GameObject screenGrup in _screenGroups)
            screenGrup.SetActive(false);

        _startScreenGroup.SetActive(true);
        _startScreen.SetActive(true);
        _currentScreenGrup = _startScreenGroup;
        _currentScreen = _startScreen;

        _currentWheatCount = _wheatCount;
        _currentPeasantCount = _peasantCount;
        _currentWarriorsCount = _warriorsCount;
    }

    public void ResetGamePlay()
    {
        _isPaused = false;
        _gamePlay = true;
        Time.timeScale = 1;

        foreach (Timer timer in _allTimers)
            timer.ResetTimer();

        _wave = 0;
        _raidersCount = 0;
        _currentWheatCount = _wheatCount;
        _currentPeasantCount = _peasantCount;
        _currentWarriorsCount = _warriorsCount;

        _resoucersText.text = $"{_peasantCount}\r\n\r\n{_warriorsCount}\r\n\r\n{_wheatCount}";
        _waveText.text = $"Wave\n{_wave}";
        _raidersText.text = $"Raiders\n{_raidersCount}";
    }

    void UpdateResourceCount()
    {
        if (_harvestTimer.Tick)
        {
            _currentWheatCount += _currentPeasantCount * _wheatOutput;
            _harvestedWheat += _currentPeasantCount * _wheatOutput;
            _harvestAudio.Play();
        }
        if (_eatingTimer.Tick)
        {
            _currentWheatCount -= _currentWarriorsCount * _wheatForCountWarrior;
            _timerEatingAudio.Play();
        }
        if (_peasantTimer.Tick)
        {
            _currentPeasantCount++;
        }
        if (_warriorTimer.Tick)
        {
            _hiredWarriors++;
            _currentWarriorsCount++;
        }
        _resoucersText.text = $"{_currentPeasantCount}\r\n\r\n{_currentWarriorsCount}\r\n\r\n{_currentWheatCount}";
    }

    void UpdateWave()
    {
        if (_raidTimer.Tick)
        {
            _raidAudio.Play();
            _maxWarriors = Mathf.Max(_maxWarriors, _currentWarriorsCount);
            _currentWarriorsCount -= _raidersCount;
            _deadRaiders += _currentWarriorsCount >= 0 ? _raidersCount : _raidersCount + _currentWarriorsCount;
            _raidersCount += _wave > 2 ? 1 : 0;
            _wave++;
            _waveText.text = $"Wave\n{_wave}";
            _raidersText.text = $"Raiders\n{_raidersCount}";
        }
    }

    public void CreatePeasant()
    {
        _currentWheatCount -= _peasantCost;
        _peasantTimer.StartTimer();
    }
    
    public void CreateWarrior()
    {
        _currentWheatCount -= _warriorsCost;
        _warriorTimer.StartTimer();
    }

    void CheckGameOverConditions()
    {
        bool isGameOver = false;
        if (_currentWheatCount < 0 || _currentWarriorsCount < 0) {
            isGameOver = true;
            _gameResultTextResult.text = "Lose";
            if (_currentWheatCount < 0)
                _gameResultText.text = "Unfortunately you ran out of food and everyone starved to death.";
            if (_currentWarriorsCount < 0)
                _gameResultText.text = "Your army has been defeated, the village has been captured."; 
        }
        if (_wave == 10) {
            isGameOver = true;
            _gameResultTextResult.text = "Win";
            _gameResultText.text = "Congratulations, you have survived 10 rounds, the backup came to you and helped you defeat the remaining enemies.";
        }
        if (isGameOver) {
            _gameResultText.text = _gameResultText.text + $"\n    Stats:\n Waves held out - {_wave}\n Warriors hired - {_hiredWarriors}\n Raiders killed - {_deadRaiders}\n Harvested wheat - {_harvestedWheat}\n Maximum warriors - {_maxWarriors}";
            _gamePlay = false;
            StateMachine(_gameResult);
        }
    }

    public void SoundControl()
    {
        _sound = !_sound;
        _audioListener.enabled = _sound;

        Texture newTexture = _sound ? _soundImageOn : _soundImageOff;

        foreach (var rawImage in _soundImage)
            rawImage.texture = newTexture;
    }

    void CheckEnoughMoney() 
    {
        _peasantButton.interactable = _currentWheatCount >= _peasantCost && !_peasantTimer.IsActive;
        _warriorButton.interactable = _currentWheatCount >= _warriorsCost && !_warriorTimer.IsActive;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}