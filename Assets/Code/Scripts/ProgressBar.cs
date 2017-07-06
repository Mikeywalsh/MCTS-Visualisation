using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// A UI element which shows a progress bar which can be customised
/// Text can also be displayed across the bar
/// </summary>
public class ProgressBar : MonoBehaviour {

    /// <summary>
    /// The actual bar rect transform, which will change size depending on the progress value
    /// </summary>
    public RectTransform progressBar;

    /// <summary>
    /// The <see cref="Text"/> behaviour attached to this progress bars child transform
    /// </summary>
    public Text progressText;

    /// <summary>
    /// A value between 0-1 which controls how much of the progress bar is displayed
    /// </summary>
    private float progress;

    /// <summary>
    /// Set the progress value of the progress bar and adjust the size of the bar appropriately
    /// </summary>
    /// <param name="newProgress">The value of the new progress to set</param>
    public void SetProgress(float newProgress)
    {
        //Ensure that the new progress value is between 0 and 1
        progress = Mathf.Clamp(newProgress, 0, 1);

        //Change the size of the progress bar transform
        float barWidth = GetComponent<RectTransform>().sizeDelta.x;
        progressBar.offsetMax = new Vector2((-1 + progress) * barWidth, progressBar.offsetMax.y);
    }

    /// <summary>
    /// Sets the currentlly displayed text on the progress bar to be equal to the passed in string
    /// </summary>
    /// <param name="text">The new text to display on the progress bar</param>
	public void SetText(string text)
    {
        progressText.text = text;
    }

    /// <summary>
    /// Clears the currently displayed text on the progress bar
    /// </summary>
    public void ClearText()
    {
        progressText.text = "";
    }

    /// <summary>
    /// The <see cref="Text"/> behaviour attached to this progress bars child transform
    /// </summary>
    public float Progress
    {
        get { return progress; }
    }
}
