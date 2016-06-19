using UnityEngine;
using System.Collections;
using LMWidgets;
using UnityEngine.UI;

public class MapLeftToggle : ButtonToggleBase 
{
  public ButtonDemoGraphics onGraphics;
  public ButtonDemoGraphics offGraphics;
  public ButtonDemoGraphics midGraphics;
  public ButtonDemoGraphics botGraphics;
  
  public Color MidGraphicsOnColor = new Color(0.0f, 0.5f, 0.5f, 1.0f);
  public Color BotGraphicsOnColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
  public Color MidGraphicsOffColor = new Color(0.0f, 0.5f, 0.5f, 0.1f);
  public Color BotGraphicsOffColor = new Color(0.0f, 0.25f, 0.25f, 1.0f);

    public Slider s;
    private float pressedTime=0;
    int updateLimit = 5;
    int updateNow = 0;

  public override void ButtonTurnsOn()
  {
        Debug.Log("TurnsOn");
        TurnsOnGraphics();
  }

  public override void ButtonTurnsOff()
  {
        Debug.Log("TurnsOff");
        TurnsOffGraphics();
  }

  private void TurnsOnGraphics()
  {
        PlayerPrefs.SetInt("nowDiff", PlayerPrefs.GetInt("nowDiff") + 1);
        onGraphics.SetActive(true);
    offGraphics.SetActive(false);
	midGraphics.SetColor(MidGraphicsOnColor);
	botGraphics.SetColor(BotGraphicsOnColor);
  }

  private void TurnsOffGraphics()
  {
        PlayerPrefs.SetInt("nowDiff", PlayerPrefs.GetInt("nowDiff") + 1);
    onGraphics.SetActive(false);
    offGraphics.SetActive(true);
	midGraphics.SetColor(MidGraphicsOffColor);
	botGraphics.SetColor(BotGraphicsOffColor);
  }

  private void UpdateGraphics()
  {
    Vector3 position = transform.localPosition;
    position.z = Mathf.Min(position.z, m_localTriggerDistance);
    onGraphics.transform.localPosition = position;
    offGraphics.transform.localPosition = position;
    Vector3 bot_position = position;
    bot_position.z = Mathf.Max(bot_position.z, m_localTriggerDistance - m_localCushionThickness);
    botGraphics.transform.localPosition = bot_position;
    Vector3 mid_position = position;
    mid_position.z = (position.z + bot_position.z) / 2.0f;
    midGraphics.transform.localPosition = mid_position;
  }

  protected override void Start()
  {
    base.Start();
  }

  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    UpdateGraphics();
        if (m_isPressed)
        {
            if (pressedTime == 0) SliderToLeft();
            pressedTime += Time.deltaTime;
            if (pressedTime >= 0.5f)
            {
                if (updateNow < updateLimit)
                    updateNow++;
                else
                {
                    updateNow = 0;
                    SliderToLeft();
                }
            }
        }
        else pressedTime = 0;
  }

    void SliderToLeft()
    {
        GameObject sliderMapMaker = GameObject.Find("MapMaker");
        if (sliderMapMaker != null)
        {
            MapMaker m = sliderMapMaker.GetComponent<MapMaker>();
            m.beatBack();
        }

        GameObject sliderTop = GameObject.Find("DemoSlider/Slider Top");
        if(sliderTop!=null)
        {
            Debug.Log(1);
            GameObject sliderLowerLimit = GameObject.Find("DemoSlider/SliderLowerLimit");
            GameObject sliderUpperLimit = GameObject.Find("DemoSlider/SliderUpperLimit");
            Transform tSliderTop = sliderTop.GetComponent<Transform>();
            Transform tSliderLowerLimit = sliderLowerLimit.GetComponent<Transform>();
            Transform tSliderUpperLimit = sliderUpperLimit.GetComponent<Transform>();
            tSliderTop.position = tSliderLowerLimit.position + (tSliderUpperLimit.position - tSliderLowerLimit.position)*(s.value/s.maxValue);
            
        }

        base.FireButtonStart(m_toggleState);
    }
}
