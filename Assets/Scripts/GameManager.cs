using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject main_image;
    public Sprite game_over_sprite;
    public Sprite game_clear_sprite;
    public GameObject panel;
    public GameObject restartButton;
    public GameObject nextButton;

    Image image;

    // =====Time Controller=====
    public GameObject time_bar;
    public GameObject timetext;
    TimeController timeController;

    private void Start()
    {
        timeController = GetComponent<TimeController>();

        if (timeController != null && timeController.game_time == 0.0f) time_bar.SetActive(false); // 시간 제한이 없을 경우

        // 내용 텍스트와 패널에 대한 설정
        // 1초 뒤에 해당 함수 호출
        Invoke("InactiveImage", 1.0f);
        // 패널 비활성화
        panel.SetActive(false);
    }

    void InactiveImage()
    {
        main_image.SetActive(false);
    }

    private void Update()
    {
        if (PlayerController.state == "gameclear")
        {
            main_image.SetActive(true); // 이미지 활성화
            panel.SetActive(true);      // 패널 활성화
            // 다시 시작 버튼에 대한 비활성화
            restartButton.GetComponent<Button>().interactable = false;
            // 메인 이미지를 게임 클리어 이미지로 변경
            main_image.GetComponent<Image>().sprite = game_clear_sprite;
            // 플레이어 컨트롤러의 상태를 end로 변경
            PlayerController.state = "end";

            if (timeController != null) timeController.is_timeover = true;

        }
        else if (PlayerController.state == "gameover")
        {
            main_image.SetActive(true); // 이미지 활성화
            panel.SetActive(true);      // 패널 활성화
            // 다음 버튼에 대한 비활성화
            nextButton.GetComponent<Button>().interactable = false;
            // 메인 이미지를 게임 클리어 이미지로 변경
            main_image.GetComponent<Image>().sprite = game_over_sprite;
            // 플레이어 컨트롤러의 상태를 end로 변경
            PlayerController.state = "end";

            if (timeController != null) timeController.is_timeover = true;

        }
        else if (PlayerController.state == "playing")
        {
            // 게임 진행에 대한 처리 구현

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            PlayerController playerController = player.GetComponent<PlayerController>();

            if (timeController != null)
            {
                if (timeController.game_time > 0.0f)
                {
                    // 정수로 세팅 소수점 버림
                    int time = (int)timeController.display_time;
                    // 시간 갱신(UI)
                    timetext.GetComponent<Text>().text = time.ToString();

                    if (time == 0) playerController.GameoOver();
                }
            }
        }
    }
}
