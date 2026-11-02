using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleUIController : MonoBehaviour
{
	private const string BACKGROUND_VIDEO_FILE = "LemonSlice_title.mp4";

	[SerializeField] private Button _startButton;
	[SerializeField] private Button _exitButton;
	[SerializeField] private Button _creditButton;
	[SerializeField] private Button _closeCreditButton;
	[SerializeField] private GameObject _creditPanel;
	[SerializeField] private VideoPlayer _backgroundVideo;

	private void Start()
	{
		OffCreditPanel();
		PlayBackgroundVideo();
	}

	private void OnEnable() => BindButtonEvents();

	private void OnDisable() => UnBindButtonEvents();

	private void PlayBackgroundVideo()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		// WebGL은 VideoClip 소스를 지원하지 않으므로 StreamingAssets URL로 재생
		_backgroundVideo.source = VideoSource.Url;
		_backgroundVideo.url = $"{Application.streamingAssetsPath}/{BACKGROUND_VIDEO_FILE}";
		// 브라우저 자동 재생 정책: 소리가 있는 영상은 사용자 입력 전 재생이 차단됨
		_backgroundVideo.SetDirectAudioMute(0, true);
#endif
		_backgroundVideo.errorReceived += OnVideoError;
		_backgroundVideo.prepareCompleted += OnVideoPrepared;
		_backgroundVideo.Prepare();
	}

	private void OnVideoPrepared(VideoPlayer videoPlayer) => videoPlayer.Play();

	private void OnVideoError(VideoPlayer videoPlayer, string message) =>
		Debug.LogError($"[TitleUIController] 배경 동영상 재생 실패: {message}");

	private void BindButtonEvents()
	{
		_startButton.onClick.AddListener(LoadMainGame);
		_creditButton.onClick.AddListener(OnCreditPanel);
		_closeCreditButton.onClick.AddListener(OffCreditPanel);
		_exitButton.onClick.AddListener(QuitGame);
	}

	private void UnBindButtonEvents()
	{
		_startButton.onClick.RemoveListener(LoadMainGame);
		_creditButton.onClick.RemoveListener(OnCreditPanel);
		_closeCreditButton.onClick.RemoveListener(OffCreditPanel);
		_exitButton.onClick.RemoveListener(QuitGame);
	}

	private void LoadMainGame()
	{
		SceneManager.LoadScene("Tutorial");
	}

	private void OnCreditPanel()
	{
		_creditPanel.SetActive(true);
	}

	private void OffCreditPanel()
	{
		_creditPanel.SetActive(false);
	}

	private void QuitGame()
	{
		Application.Quit();
	}
}
