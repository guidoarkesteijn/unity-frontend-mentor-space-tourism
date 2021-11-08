using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

public class LocalizedLabel : Label
{
  private string Table { get; set; }
  private string Key { get; set; }

  private Label label;

  //
  // Summary:
  //     Instantiates a Label using the data read from a UXML file.
  public new class UxmlFactory : UxmlFactory<LocalizedLabel, UxmlTraits>
  {
    public UxmlFactory() : base()
    {

    }
  }
  //
  // Summary:
  //     Defines UxmlTraits for the Label.
  public new class UxmlTraits : TextElement.UxmlTraits
  {
    private UxmlStringAttributeDescription table = new UxmlStringAttributeDescription
    {
      name = "table",
      defaultValue = "<TABLE>"
    };

    private UxmlStringAttributeDescription key = new UxmlStringAttributeDescription
    {
      name = "key",
      defaultValue = "<KEY>"
    };

    /// <summary>
    ///   <para>Enumerator to get the child elements of the UxmlTraits of TextElement.</para>
    /// </summary>
    public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
    {
      get
      {
        yield break;
      }
    }

    /// <summary>
    ///   <para>Initializer for the UxmlTraits for the TextElement.</para>
    /// </summary>
    /// <param name="ve">VisualElement to initialize.</param>
    /// <param name="bag">Bag of attributes where to get the value from.</param>
    /// <param name="cc">Creation Context, not used.</param>
    public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
    {
      base.Init(ve, bag, cc);
      LocalizedLabel localizedLabel = ((LocalizedLabel)ve);

      localizedLabel.Table = table.GetValueFromBag(bag, cc);
      localizedLabel.Key = key.GetValueFromBag(bag, cc);

      localizedLabel.UpdateText();
    }
  }

  public LocalizedLabel() : this(string.Empty, string.Empty) { }

  public LocalizedLabel(string table, string key)
  {
    label = this.Q<Label>();

    Table = table;
    Key = key;

    RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
    RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
  }

  private void OnAttachToPanelEvent(AttachToPanelEvent attachToPanelEvent)
  {
    UpdateText();
    Subscribe();
  }

  private void OnDetachFromPanelEvent(DetachFromPanelEvent detachFromPanelEvent)
  {
    UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
    UnregisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);

    Unsubscribe();
  }

  private void Subscribe()
  {
    if (Application.isPlaying)
    {
      LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChangedEvent;
      LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChangedEvent;
    }
  }

  private void Unsubscribe()
  {
    if (Application.isPlaying)
    {
      LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChangedEvent;
    }
  }

  private void OnSelectedLocaleChangedEvent(UnityEngine.Localization.Locale obj)
  {
    UpdateText();
  }

  public void UpdateText()
  {
    string result = Table + "/" + Key;

    if (Application.isPlaying && !string.IsNullOrEmpty(Table) && !string.IsNullOrEmpty(Key))
    {
      result = LocalizationSettings.StringDatabase.GetLocalizedString(Table, Key);
    }

    SetTextElement(result);
  }

  private void SetTextElement(string value)
  {
    label.text = value;
  }
}
