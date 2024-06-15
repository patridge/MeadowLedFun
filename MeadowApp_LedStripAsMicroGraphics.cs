#nullable enable
using Meadow;
using Meadow.Devices;
// using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
// using Meadow.Units;
// using System;
using System.Numerics;
// using System.Runtime.CompilerServices;
// using System.Threading;
using System.Threading.Tasks;

namespace LedFun;

// Change F7FeatherV2 to F7FeatherV1 if using Feather V1 Meadow boards
// Change to F7CoreComputeV2 for Project Lab V3.x
public class MeadowApp_LedStripAsMicroGraphics : App<F7CoreComputeV2>
{
    IProjectLabHardware? projectLab;
    DisplayController? displayController;
    MicroGraphics? graphics;
    Apa102? apa102;
    const int numberOfLeds = 15;
    const float maxBrightness = 0.001f;
    int cursorLocation = 0;
    Color cursorColor = Color.Red;
    Vector3 angle = new Vector3(0, 0, 0);
    bool handleGyroscope = true;

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
        graphics = new MicroGraphics(apa102);

        Resolver.Log.Info("Initialization complete");
        return base.Initialize();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        Resolver.Log.Info("starting blink");

        graphics!.PenColor = Color.Blue;
        // graphics.DrawLine(1, 0, 5, 0);
        graphics.DrawLine(0, 0, 5, 5);
        // graphics.DrawCircle(0, 0, 100, filled: true);
        graphics.Show();

        return base.Run();
    }
}