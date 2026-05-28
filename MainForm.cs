using System.Drawing.Drawing2D;

namespace MangoExecutor;

public sealed class MainForm : Form
{
    private readonly TabControl tabs = new();
    private readonly TabPage editorTab = new("Editor");
    private readonly TabPage settingsTab = new("Settings");
    private readonly TabPage scriptHubTab = new("Script Hub");
    private readonly RichTextBox editor = new();
    private readonly NeonLabel titleLabel = new();
    private readonly NeonLabel statusLabel = new();
    private readonly Button executeButton = new();
    private readonly Button injectButton = new();
    private readonly Button saveButton = new();
    private readonly Button customColorButton = new();
    private readonly ComboBox themeSelector = new();

    private readonly Color background = Color.FromArgb(8, 9, 12);
    private readonly Color surface = Color.FromArgb(16, 18, 23);
    private readonly Color raisedSurface = Color.FromArgb(23, 25, 31);
    private readonly Color mutedText = Color.FromArgb(155, 163, 176);
    private Color accent = Color.FromArgb(255, 128, 34);

    public MainForm()
    {
        Text = "Mango Executor";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 560);
        Size = new Size(1100, 700);
        Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        BackColor = background;
        ForeColor = Color.White;

        BuildLayout();
        ApplyTheme(accent);
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(22),
            BackColor = background,
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = background,
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));

        titleLabel.Text = "MANGO EXECUTOR";
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;

        statusLabel.Text = "Ready";
        statusLabel.Dock = DockStyle.Fill;
        statusLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        statusLabel.TextAlign = ContentAlignment.MiddleRight;

        header.Controls.Add(titleLabel, 0, 0);
        header.Controls.Add(statusLabel, 1, 0);

        tabs.Dock = DockStyle.Fill;
        tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabs.ItemSize = new Size(128, 38);
        tabs.SizeMode = TabSizeMode.Fixed;
        tabs.Padding = new Point(16, 6);
        tabs.DrawItem += Tabs_DrawItem;
        tabs.Controls.Add(editorTab);
        tabs.Controls.Add(settingsTab);
        tabs.Controls.Add(scriptHubTab);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(tabs, 0, 1);
        Controls.Add(root);

        BuildEditorTab();
        BuildSettingsTab();
        BuildScriptHubTab();
    }

    private void BuildEditorTab()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0, 18, 0, 0),
            BackColor = background,
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));

        var editorFrame = new NeonPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(14),
            BackColor = surface,
        };

        editor.Dock = DockStyle.Fill;
        editor.BorderStyle = BorderStyle.None;
        editor.Font = new Font("Consolas", 11.5F, FontStyle.Regular);
        editor.AcceptsTab = true;
        editor.WordWrap = false;
        editor.ScrollBars = RichTextBoxScrollBars.Both;
        editor.Text = "-- Write your script here";
        editor.BackColor = Color.FromArgb(10, 11, 15);
        editor.ForeColor = Color.FromArgb(255, 177, 94);
        editor.SelectionColor = editor.ForeColor;

        editorFrame.Controls.Add(editor);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 16, 0, 0),
            WrapContents = false,
            BackColor = background,
        };

        ConfigureButton(saveButton, "Save");
        ConfigureButton(injectButton, "Inject");
        ConfigureButton(executeButton, "Execute");

        executeButton.Click += (_, _) => ShowStatus("Execute clicked");
        injectButton.Click += (_, _) => ShowStatus("Inject clicked");
        saveButton.Click += SaveButton_Click;

        actions.Controls.Add(saveButton);
        actions.Controls.Add(injectButton);
        actions.Controls.Add(executeButton);

        layout.Controls.Add(editorFrame, 0, 0);
        layout.Controls.Add(actions, 0, 1);
        editorTab.Controls.Add(layout);
    }

    private void BuildSettingsTab()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0, 18, 0, 0),
            BackColor = background,
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var settingsFrame = new NeonPanel
        {
            Dock = DockStyle.Top,
            Height = 164,
            Padding = new Padding(22),
            BackColor = surface,
        };

        var settingsGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            BackColor = surface,
        };
        settingsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        settingsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));
        settingsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        settingsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        var themeLabel = CreateSettingsLabel("Neon color");
        themeSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        themeSelector.FlatStyle = FlatStyle.Flat;
        themeSelector.BackColor = raisedSurface;
        themeSelector.ForeColor = Color.White;
        themeSelector.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        themeSelector.Items.AddRange(new object[] { "Orange", "Hot Pink", "Electric Blue", "Lime", "Violet" });
        themeSelector.SelectedIndex = 0;
        themeSelector.SelectedIndexChanged += (_, _) => ApplySelectedTheme();

        var customLabel = CreateSettingsLabel("Custom");
        ConfigureButton(customColorButton, "Pick Color");
        customColorButton.Click += CustomColorButton_Click;

        settingsGrid.Controls.Add(themeLabel, 0, 0);
        settingsGrid.Controls.Add(themeSelector, 1, 0);
        settingsGrid.Controls.Add(customLabel, 0, 1);
        settingsGrid.Controls.Add(customColorButton, 1, 1);
        settingsFrame.Controls.Add(settingsGrid);

        layout.Controls.Add(settingsFrame, 0, 0);
        settingsTab.Controls.Add(layout);
    }

    private void BuildScriptHubTab()
    {
        scriptHubTab.BackColor = background;
        scriptHubTab.Padding = new Padding(0, 18, 0, 0);
    }

    private Label CreateSettingsLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = mutedText,
            BackColor = surface,
        };
    }

    private void ConfigureButton(Button button, string text)
    {
        button.Text = text.ToUpperInvariant();
        button.Width = 126;
        button.Height = 38;
        button.FlatStyle = FlatStyle.Flat;
        button.Margin = new Padding(10, 0, 0, 0);
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        button.UseVisualStyleBackColor = false;
    }

    private void ApplySelectedTheme()
    {
        var selectedAccent = themeSelector.SelectedItem?.ToString() switch
        {
            "Hot Pink" => Color.FromArgb(255, 48, 145),
            "Electric Blue" => Color.FromArgb(64, 190, 255),
            "Lime" => Color.FromArgb(148, 255, 80),
            "Violet" => Color.FromArgb(173, 112, 255),
            _ => Color.FromArgb(255, 128, 34),
        };

        ApplyTheme(selectedAccent);
    }

    private void ApplyTheme(Color newAccent)
    {
        accent = newAccent;
        titleLabel.GlowColor = accent;
        titleLabel.ForeColor = accent;
        statusLabel.GlowColor = accent;
        statusLabel.ForeColor = accent;

        foreach (TabPage page in tabs.TabPages)
        {
            page.BackColor = background;
            page.ForeColor = Color.White;
        }

        foreach (var panel in GetControls<NeonPanel>(this))
        {
            panel.BorderColor = accent;
            panel.GlowColor = Color.FromArgb(90, accent);
            panel.Invalidate();
        }

        editor.ForeColor = Blend(accent, Color.White, 0.22F);
        editor.SelectionColor = editor.ForeColor;
        StyleButton(executeButton, accent, filled: true);
        StyleButton(injectButton, accent, filled: false);
        StyleButton(saveButton, accent, filled: false);
        StyleButton(customColorButton, accent, filled: false);
        tabs.Invalidate();
        Invalidate();
    }

    private static void StyleButton(Button button, Color accent, bool filled)
    {
        button.BackColor = filled ? accent : Color.FromArgb(16, 18, 23);
        button.ForeColor = filled ? Color.Black : accent;
        button.FlatAppearance.BorderColor = accent;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.MouseOverBackColor = filled ? Lighten(accent, 0.12F) : Color.FromArgb(31, 25, 21);
        button.FlatAppearance.MouseDownBackColor = filled ? Darken(accent, 0.10F) : Color.FromArgb(45, 31, 21);
    }

    private void Tabs_DrawItem(object? sender, DrawItemEventArgs e)
    {
        var selected = e.Index == tabs.SelectedIndex;
        var rect = e.Bounds;
        rect.Inflate(-4, -5);

        using var backgroundBrush = new SolidBrush(selected ? Color.FromArgb(25, 27, 34) : Color.FromArgb(12, 13, 17));
        using var borderPen = new Pen(selected ? accent : Color.FromArgb(39, 42, 50), 1F);
        using var textBrush = new SolidBrush(selected ? accent : mutedText);
        using var path = RoundedRect(rect, 7);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.FillPath(backgroundBrush, path);
        e.Graphics.DrawPath(borderPen, path);

        if (selected)
        {
            using var glowPen = new Pen(Color.FromArgb(100, accent), 3F);
            e.Graphics.DrawPath(glowPen, path);
        }

        TextRenderer.DrawText(
            e.Graphics,
            tabs.TabPages[e.Index].Text.ToUpperInvariant(),
            new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
            rect,
            selected ? accent : mutedText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Save script",
            Filter = "Lua script (*.lua)|*.lua|Text file (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = "lua",
            FileName = "script.lua",
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        File.WriteAllText(dialog.FileName, editor.Text);
        ShowStatus($"Saved {Path.GetFileName(dialog.FileName)}");
    }

    private void CustomColorButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new ColorDialog
        {
            Color = accent,
            FullOpen = true,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        ApplyTheme(dialog.Color);
    }

    private void ShowStatus(string message)
    {
        statusLabel.Text = message;
    }

    private static IEnumerable<T> GetControls<T>(Control parent) where T : Control
    {
        foreach (Control control in parent.Controls)
        {
            if (control is T match)
            {
                yield return match;
            }

            foreach (var child in GetControls<T>(control))
            {
                yield return child;
            }
        }
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static Color Blend(Color first, Color second, float amount)
    {
        amount = Math.Clamp(amount, 0F, 1F);
        var inverse = 1F - amount;
        return Color.FromArgb(
            (int)((first.R * inverse) + (second.R * amount)),
            (int)((first.G * inverse) + (second.G * amount)),
            (int)((first.B * inverse) + (second.B * amount)));
    }

    private static Color Lighten(Color color, float amount)
    {
        return Blend(color, Color.White, amount);
    }

    private static Color Darken(Color color, float amount)
    {
        return Blend(color, Color.Black, amount);
    }
}

public sealed class NeonLabel : Label
{
    public Color GlowColor { get; set; } = Color.FromArgb(255, 128, 34);

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        for (var offset = 4; offset >= 1; offset--)
        {
            using var glowBrush = new SolidBrush(Color.FromArgb(28, GlowColor));
            var glowRect = new Rectangle(offset, offset, Width - (offset * 2), Height - (offset * 2));
            TextRenderer.DrawText(e.Graphics, Text, Font, glowRect, GlowColor, TextFlags);
        }

        TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, TextFlags);
    }

    private TextFormatFlags TextFlags
    {
        get
        {
            var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
            flags |= TextAlign switch
            {
                ContentAlignment.MiddleRight => TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                ContentAlignment.MiddleCenter => TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ContentAlignment.MiddleLeft => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                _ => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
            };
            return flags;
        }
    }
}

public sealed class NeonPanel : Panel
{
    public Color BorderColor { get; set; } = Color.FromArgb(255, 128, 34);
    public Color GlowColor { get; set; } = Color.FromArgb(90, 255, 128, 34);

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;

        using var path = RoundedRect(rect, 8);
        using var glowPen = new Pen(GlowColor, 4F);
        using var borderPen = new Pen(BorderColor, 1.2F);
        e.Graphics.DrawPath(glowPen, path);
        e.Graphics.DrawPath(borderPen, path);
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}
