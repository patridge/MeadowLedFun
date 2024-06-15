// #nullable enable
// using Meadow;
// using Meadow.Devices;
// // using Meadow.Foundation;
// // using Meadow.Foundation.Graphics;
// using Meadow.Foundation.Leds;
// using Meadow.Units;
// using System;
// using System.Numerics;
// // using System.Runtime.CompilerServices;
// using System.Threading;
// using System.Threading.Tasks;

// namespace LedFun;

// // Change F7FeatherV2 to F7FeatherV1 if using Feather V1 Meadow boards
// // Change to F7CoreComputeV2 for Project Lab V3.x
// public class MeadowApp_OldGyroAngleMath : App<F7CoreComputeV2>
// {
//     enum AnimationDirection
//     {
//         Forward,
//         Backward
//     }

//     IProjectLabHardware? projectLab;
//     Apa102? apa102;
//     const int numberOfLeds = 15;
//     const float maxBrightness = 0.001f;
//     int cursorLocation = 0;
//     AnimationDirection animationDirection = AnimationDirection.Forward;
//     Color cursorColor = Color.Red;
//     Vector3 angle = new Vector3(0, 0, 0);
//     readonly TimeSpan updateTime = TimeSpan.FromMilliseconds(100);
//     bool handleGyroscope = true;
//     ILedDisplay ledDisplay = new MeteorTrailDisplay(numberOfLeds);

//     public override Task Initialize()
//     {
//         Resolver.Log.LogLevel = Meadow.Logging.LogLevel.Trace;
//         Resolver.Log.Info("Initialize hardware...");

//         projectLab = ProjectLab.Create();

//         Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

//         if (projectLab.RgbLed is { } rgbLed)
//         {
//             rgbLed.SetColor(Color.Blue);
//         }

//         apa102 = new Apa102(Device.CreateSpiBus(Device.Pins.SPI5_SCK, Device.Pins.SPI5_COPI, Device.Pins.SPI5_CIPO), numberOfLeds, Apa102.PixelOrder.BGR);
//         apa102!.Brightness = maxBrightness;

//         if (projectLab.UpButton is { } upButton)
//         {
//             upButton.Clicked += (s, e) => {
//                 if (cursorColor == Color.Red) { cursorColor = Color.Green; }
//                 else if (cursorColor == Color.Green) { cursorColor = Color.Blue; }
//                 else { cursorColor = Color.Red; }
//                 DrawLights(ledDisplay);
//             };
//         }
//         if (projectLab.DownButton is { } downButton)
//         {
//             downButton.Clicked += (s, e) => {
//                 if (cursorColor == Color.Red) { cursorColor = Color.Green; }
//                 else if (cursorColor == Color.Green) { cursorColor = Color.Blue; }
//                 else { cursorColor = Color.Red; }
//                 DrawLights(ledDisplay);
//             };

//             // With the old system of angle math, it would occasionally drift. This was a way to reset it.
//             downButton.LongClicked += (s, e) => {
//                 angle = new Vector3(0, 0, 0);
//                 // Blink the cursor
//                 for (int i = 0; i < 3; i++)
//                 {
//                     apa102!.Clear();
//                     apa102.Show();
//                     Thread.Sleep(200);
//                     DrawLights(ledDisplay);
//                     Thread.Sleep(200);
//                 }
//             };
//         }
//         if (projectLab.LeftButton is { } leftButton)
//         {
//             leftButton.Clicked += (s, e) => {
//                 cursorLocation -= 1;
//                 if (cursorLocation < 0) { cursorLocation = 0; }
//                 DrawLights(ledDisplay);
//             };
//         }
//         if (projectLab.RightButton is { } rightButton)
//         {
//             rightButton.Clicked += (s, e) => {
//                 cursorLocation += 1;
//                 if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
//                 DrawLights(ledDisplay);
//             };
//         }
//         if (projectLab.Gyroscope is { } gyroscope)
//         {
//             gyroscope.Updated += OnGyroscopeUpdated;
//         }

//         Resolver.Log.Info("Initialization complete");
//         return base.Initialize();
//     }

//     private void OnGyroscopeUpdated(object sender, IChangeResult<AngularVelocity3D> e)
//     {
//         if (!handleGyroscope)
//         {
//             return;
//         }

//         // Resolver.Log.Info($"GYRO: {e.New.X.DegreesPerSecond:0.0}, {e.New.Y.DegreesPerSecond:0.0}, {e.New.Z.DegreesPerSecond:0.0}deg/s");
//         // Get current angle
//         var newAngle = new Vector3(
//             angle.X + (float) e.New.X.DegreesPerSecond * (float) updateTime.TotalSeconds, 
//             angle.Y + (float) e.New.Y.DegreesPerSecond * (float) updateTime.TotalSeconds, 
//             angle.Z + (float) e.New.Z.DegreesPerSecond * (float) updateTime.TotalSeconds
//         );

//         // Define a threshold for changes in the gravity angle
//         float threshold = 0.1f;

//         // If the change in the angle is less than the threshold, return early
//         if (Math.Abs(newAngle.X - angle.X) < threshold)
//         {
//             return;
//         }

//         // Resolver.Log.Info($"GYRO: {e.New.X.DegreesPerSecond:0.0}, {e.New.Y.DegreesPerSecond:0.0}, {e.New.Z.DegreesPerSecond:0.0}deg/s");
//         angle = newAngle;
//         // Resolver.Log.Info($"GYRO: {angle.X:0.0}, {angle.Y:0.0}, {angle.Z:0.0}deg");

//         if (angle.X > 10)
//         {
//             cursorLocation += 1;
//             if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
//             DrawLights(ledDisplay);
//         }
//         else if (angle.X < -10)
//         {
//             cursorLocation -= 1;
//             if (cursorLocation < 0) { cursorLocation = 0; }
//             DrawLights(ledDisplay);
//         }
//     }

//     interface ILedDisplay
//     {
//         // int NumberOfLeds { get; }
//         // // Color[] LedColors { get; }
//         // float[] LedBrightness { get; }
//         float GetLedBrightness(int cursorDeltaIndex);
//     }
//     class MeteorTrailDisplay : ILedDisplay
//     {
//         public MeteorTrailDisplay(int numberOfLeds)
//         {
//             // NumberOfLeds = numberOfTrailLeds;
//             TrailLength = numberOfLeds / 3;
//             // LedColors = new Color[NumberOfLeds];
//             // LedBrightness = new float[NumberOfLeds];
//         }

//         public int TrailLength { get; }
//         // public Color[] LedColors { get; }
//         // public float[] LedBrightness { get; }

//         /// <summary>
//         /// </summary>
//         /// <param name="cursorDeltaIndex">
//         /// Delta of current LED from current cursor LED location. A `cursorDetlaIndex` of 0 is the cursor, -1 is one behind the cursor, 2 is two ahead of the cursor, etc.
//         /// </param>
//         public float GetLedBrightness(int cursorDeltaIndex)
//         {
//             if (cursorDeltaIndex == 0)
//             {
//                 return 1.0f;
//             }
//             else if (cursorDeltaIndex > TrailLength)
//             {
//                 return 0.0f;
//             }
//             else {
//                 // For Meteor Trail, we want the cursor to be the brightest, and then trail off behind the cursor.
//                 return 1.0f / (float)Math.Pow(2, Math.Abs(cursorDeltaIndex));
//             }
//         }
//     }

//     void DrawLights(ILedDisplay ledDisplay)
//     {
//         // Resolver.Log.Trace($"ShowCursor: {cursorLocation}");
//         apa102!.Clear();
//         for (int i = 0; i < numberOfLeds; i++)
//         {
//             var brightness = ledDisplay.GetLedBrightness(i - cursorLocation);
//             apa102.SetLed(i, cursorColor, brightness);
//         }
//         apa102.Show();
//     }

//     public override Task Run()
//     {
//         Resolver.Log.Info("Run...");

//         if (projectLab!.Gyroscope is { } gyroscope)
//         {
//             gyroscope.StartUpdating(updateTime);
//         }

//         Resolver.Log.Info("starting blink");
//         DrawLights(ledDisplay);
//         return base.Run();
//     }
// }
