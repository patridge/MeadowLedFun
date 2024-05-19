#nullable enable
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Units;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace LedFun;

// Change F7FeatherV2 to F7FeatherV1 if using Feather V1 Meadow boards
// Change to F7CoreComputeV2 for Project Lab V3.x
public class MeadowApp : App<F7CoreComputeV2>
{
    IProjectLabHardware? projectLab;
    DisplayController? displayController;
    Apa102? apa102;
    readonly int numberOfLeds = 15;
    readonly float maxBrightness = 0.001f;
    // MicroGraphics graphics;
    int cursorLocation = 0;
    Color cursorColor = Color.Red;
    Vector3 angle = new Vector3(0, 0, 0);
    readonly TimeSpan updateTime = TimeSpan.FromMilliseconds(100);

    public override Task Initialize()
    {
        Resolver.Log.LogLevel = Meadow.Logging.LogLevel.Trace;
        Resolver.Log.Info("Initialize hardware...");

        projectLab = ProjectLab.Create();

        Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

        if (projectLab.RgbLed is { } rgbLed)
        {
            rgbLed.SetColor(Color.Blue);
        }

        if (projectLab.Display is { } display)
        {
            Resolver.Log.Trace("Creating DisplayController");
            displayController = new DisplayController(display, projectLab.RevisionString);
            Resolver.Log.Trace("DisplayController up");
        }

        apa102 = new Apa102(Device.CreateSpiBus(Device.Pins.SPI5_SCK, Device.Pins.SPI5_COPI, Device.Pins.SPI5_CIPO), numberOfLeds, Apa102.PixelOrder.BGR);
        apa102!.Brightness = maxBrightness;
        // graphics = new MicroGraphics(apa102);

        if (projectLab.UpButton is { } upButton)
        {
            upButton.Clicked += (s, e) => {
                if (cursorColor == Color.Red) { cursorColor = Color.Green; }
                else if (cursorColor == Color.Green) { cursorColor = Color.Blue; }
                else { cursorColor = Color.Red; }
                ShowCursor();
            };
            // upButton.PressEnded += (s, e) => displayController!.UpButtonState = false;
        }
        if (projectLab.DownButton is { } downButton)
        {
            downButton.Clicked += (s, e) => {
                if (cursorColor == Color.Red) { cursorColor = Color.Green; }
                else if (cursorColor == Color.Green) { cursorColor = Color.Blue; }
                else { cursorColor = Color.Red; }
                ShowCursor();
            };
            // downButton.PressStarted += (s, e) => displayController!.DownButtonState = true;
            // downButton.PressEnded += (s, e) => displayController!.DownButtonState = false;
            downButton.LongClicked += (s, e) => {
                angle = new Vector3(0, 0, 0);
                // Blink the cursor
                for (int i = 0; i < 3; i++)
                {
                    apa102!.Clear();
                    apa102.Show();
                    Thread.Sleep(200);
                    ShowCursor();
                    Thread.Sleep(200);
                }
            };
        }
        if (projectLab.LeftButton is { } leftButton)
        {
            leftButton.Clicked += (s, e) => {
                cursorLocation -= 1;
                if (cursorLocation < 0) { cursorLocation = 0; }
                ShowCursor();
            };
            // leftButton.PressStarted += (s, e) => displayController!.LeftButtonState = true;
            // leftButton.PressEnded += (s, e) => displayController!.LeftButtonState = false;
        }
        if (projectLab.RightButton is { } rightButton)
        {
            rightButton.Clicked += (s, e) => {
                cursorLocation += 1;
                if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
                ShowCursor();
            };
            // rightButton.PressStarted += (s, e) => displayController!.RightButtonState = true;
            // rightButton.PressEnded += (s, e) => displayController!.RightButtonState = false;
        }
        if (projectLab.Gyroscope is { } gyroscope)
        {
            gyroscope.Updated += OnGyroscopeUpdated;
        }

        Resolver.Log.Info("Initialization complete");
        return base.Initialize();
    }

    private void OnGyroscopeUpdated(object sender, IChangeResult<AngularVelocity3D> e)
    {
        // Resolver.Log.Info($"GYRO: {e.New.X.DegreesPerSecond:0.0}, {e.New.Y.DegreesPerSecond:0.0}, {e.New.Z.DegreesPerSecond:0.0}deg/s");
        // displayController.GyroConditions = e.New;
        // Get current angle
        var newAngle = new Vector3(
            angle.X + (float) e.New.X.DegreesPerSecond * (float) updateTime.TotalSeconds, 
            angle.Y + (float) e.New.Y.DegreesPerSecond * (float) updateTime.TotalSeconds, 
            angle.Z + (float) e.New.Z.DegreesPerSecond * (float) updateTime.TotalSeconds);
        // Resolver.Log.Info($"GYRO: {e.New.X.DegreesPerSecond:0.0}, {e.New.Y.DegreesPerSecond:0.0}, {e.New.Z.DegreesPerSecond:0.0}deg/s");
        angle = newAngle;
        // Resolver.Log.Info($"GYRO: {angle.X:0.0}, {angle.Y:0.0}, {angle.Z:0.0}deg");

        if (angle.X > 10)
        {
            cursorLocation += 1;
            if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
            ShowCursor();
        }
        else if (angle.X < -10)
        {
            cursorLocation -= 1;
            if (cursorLocation < 0) { cursorLocation = 0; }
            ShowCursor();
        }
    }
    void ShowCursor()
    {
        apa102!.Clear();
        // Resolver.Log.Info($"ShowCursor: {cursorLocation}");
        apa102.SetLed(cursorLocation, cursorColor);
        apa102.Show();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        if (projectLab!.Gyroscope is { } gyroscope)
        {
            gyroscope.StartUpdating(updateTime);
        }
        // displayController?.Update();

        Resolver.Log.Info("starting blink");
        // _ = projectLab!.RgbLed?.StartBlink(WildernessLabsColors.PearGreen, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2000), 0.5f);

        // Set each LED to red, green, and blue, cycling in order.
        // Color currentColor = Color.Red;
        // apa102!.Clear();
        // for (int i = 0; i < numberOfLeds; i++)
        // {
        //     apa102.SetLed(i, currentColor);
        //     if (currentColor == Color.Red) { currentColor = Color.Green; }
        //     else if (currentColor == Color.Green) { currentColor = Color.Blue; }
        //     else { currentColor = Color.Red; }
        // }
        // apa102.Show();

        ShowCursor();

        // graphics.PenColor = Color.Red;
        // graphics.DrawLine(1, 0, 5, 0);
        // graphics.DrawCircle(0, 0, 100, filled: true);
        // graphics.Show();

        // apa102.Show();

        // Apa102Tests();

        return base.Run();
    }

    void Apa102Tests()
    {
        while (true)
        {
            SetColor(Colors.ChileanFire, maxBrightness);
            Thread.Sleep(1000);
            SetColor(Colors.PearGreen, maxBrightness);
            Thread.Sleep(1000);
            Pulse(Colors.AzureBlue, 10);
            WalkTheStrip(Colors.ChileanFire, 10);
        }
    }

    /// <summary>
    /// Sets the entire strip to be one color.
    /// </summary>
    /// <param name="color"></param>
    void SetColor(Color color, float brightness)
    {
        Resolver.Log.Info($"SetColor(color:{color}");

        for (int i = 0; i < apa102!.NumberOfLeds; i++)
        {
            apa102.SetLed(i, color, brightness);
        }
        apa102.Show();
    }

    /// <summary>
    /// pulses the entire strip up and down in brightness
    /// </summary>
    /// <param name="color"></param>
    void Pulse(Color color, int numberOfPulses)
    {
        Resolver.Log.Info("Pulse");

        float minBrightness = 0.05f;
        float brightness = minBrightness;
        float increment = 0.01f; // the colors don't seem to have more resolution than this.
        bool forward = true;

        int pulsesPerLoop = (int)(maxBrightness / increment * 2);
        int totalNumberOfPulses = numberOfPulses * pulsesPerLoop;

        for (int loop = 0; loop < totalNumberOfPulses; loop++)
        {
            // increment/decrement our brightness depending on direction
            if (forward) { brightness += increment; }
            else { brightness -= increment; }

            if (brightness <= minBrightness)
            {
                forward = true;
            }
            if (brightness >= maxBrightness)
            {
                forward = false;
            }

            // set all the leds one color
            for (int i = 0; i < apa102!.NumberOfLeds; i++)
            {
                apa102.SetLed(i, color, brightness);
            }

            apa102.Show();

            Thread.Sleep(10);
        }
    }

    /// <summary>
    /// Walks a lighted LED, up and down the strip.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="numberOfTraverses"></param>
    void WalkTheStrip(Color color, int numberOfTraverses)
    {
        int last = numberOfLeds - 1;

        bool forward = true;
        int index = 0;

        for (int loop = 0; loop < numberOfTraverses * apa102!.NumberOfLeds * 2; loop++)
        {
            if (last != 9999) { apa102.SetLed(last, Color.Black); }
            apa102.SetLed(index, color);
            last = index;

            if (forward) { index++; }
            else { index--; }

            apa102.Show();

            if (index == apa102.NumberOfLeds - 1) { forward = false; }
            if (index == 0) { forward = true; }

            Thread.Sleep(50);
        }
    }

    void Start()
    {
        Resolver.Log.Info("Run...");
        apa102!.Clear();
        apa102.Show();
        Thread.Sleep(2000);

        apa102.SetLed(0, Color.Red, 0.5f);
        apa102.SetLed(1, Color.White);
        apa102.SetLed(2, Color.Blue);
        apa102.Show();
        Thread.Sleep(2000);

        apa102.SetLed(0, Color.Green);
        apa102.SetLed(1, Color.Yellow);
        apa102.SetLed(2, Color.Pink);
        apa102.Show();
        Thread.Sleep(5000);

        apa102.Clear(true);
    }

    public static class Colors
    {
        public static Color AzureBlue = Color.FromHex("#23abe3");
        public static Color ChileanFire = Color.FromHex("#EF7D3B");
        public static Color PearGreen = Color.FromHex("#C9DB31");
    }
}