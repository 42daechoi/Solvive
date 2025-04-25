using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionControl : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;
    public Button applyButton;   // "확인" 혹은 "Apply" 버튼

    private Resolution[] resolutions;

    // 사용자가 드롭다운/토글로 선택한 값(실제 적용 전까지 저장만 함)
    private int selectedResolutionIndex;
    private bool isFullScreenSelected;

    void Start()
    {
        // 1. 해상도 목록 가져오기 (중복 제거)
        Resolution[] allResolutions = Screen.resolutions;
        List<Resolution> uniqueResolutions = new List<Resolution>();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            string option = $"{allResolutions[i].width} x {allResolutions[i].height}";
            if (!options.Contains(option))
            {
                options.Add(option);
                uniqueResolutions.Add(allResolutions[i]);

                // 현재 해상도 위치 저장
                if (allResolutions[i].width == Screen.currentResolution.width &&
                    allResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }
        }

        // 2. 최종 해상도 배열로 정리
        resolutions = uniqueResolutions.ToArray();

        // 3. 드롭다운 초기화
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // 4. 토글 초기값
        fullScreenToggle.isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow);

        // 5. 현재 UI에서 선택된 값을 내부 변수에 저장
        selectedResolutionIndex = resolutionDropdown.value;
        isFullScreenSelected = fullScreenToggle.isOn;

        // 6. 리스너 등록
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullScreenToggle.onValueChanged.AddListener(OnFullScreenToggle);
        applyButton.onClick.AddListener(OnClickApply);
    }

    // 드롭다운이 바뀔 때마다 내부 변수만 업데이트
    private void OnResolutionChanged(int value)
    {
        selectedResolutionIndex = value;
    }

    // 토글이 바뀔 때마다 내부 변수만 업데이트
    private void OnFullScreenToggle(bool isOn)
    {
        isFullScreenSelected = isOn;
    }

    // "확인(Apply)" 버튼을 눌렀을 때만 실제 적용
    private void OnClickApply()
    {
        Resolution selectedResolution = resolutions[selectedResolutionIndex];
        FullScreenMode mode = isFullScreenSelected
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        // 실제로 해상도, 전체화면 모드 변경
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, mode);
    }
}
