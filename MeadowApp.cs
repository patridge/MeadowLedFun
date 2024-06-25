#nullable enable
using Meadow;
using Meadow.Devices;
// using Meadow.Foundation;
// using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
// using Meadow.Hardware;
// using Meadow.Peripherals.Leds;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
// using System.Runtime.CompilerServices;
// using System.Threading;
using System.Threading.Tasks;

namespace LedFun;

// Change F7FeatherV2 to F7FeatherV1 if using Feather V1 Meadow boards
// Change to F7CoreComputeV2 for Project Lab V3.x
public class MeadowApp : App<F7CoreComputeV2>
{
    enum AnimationDirection
    {
        Forward,
        Backward
    }

    IProjectLabHardware? projectLab;
    Apa102? apa102;
    const int numberOfLeds = 15;
    const float maxBrightness = 0.25f;
    float tiltAngleThreshold = 0.25f;
    readonly TimeSpan sensorUpdateTime = TimeSpan.FromMilliseconds(100);
    // ILedDisplay ledDisplay = new RobotEyeDisplay(numberOfLeds);
    SnakeDisplay snakeDisplay = new SnakeDisplay((int)Math.Floor(numberOfLeds / 4.0), Color.Red, maxBrightness);

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

        // if (projectLab.Display is { } display)
        // {
        //     Resolver.Log.Trace("Creating DisplayController");
        //     displayController = new DisplayController(display, projectLab.RevisionString);
        //     Resolver.Log.Trace("DisplayController up");
        // }

        apa102 = new Apa102(Device.CreateSpiBus(Device.Pins.SPI5_SCK, Device.Pins.SPI5_COPI, Device.Pins.SPI5_CIPO), numberOfLeds, Apa102.PixelOrder.BGR);
        apa102!.Brightness = maxBrightness;

        if (projectLab.UpButton is { } upButton)
        {
            // upButton.Clicked += (s, e) => {
            //     if (cursorColor == Color.Red) { cursorColor = Color.Green; }
            //     else if (cursorColor == Color.Green) { cursorColor = Color.Blue; }
            //     else { cursorColor = Color.Red; }
            //     DrawLights(ledDisplay);
            // };
        }
        if (projectLab.DownButton is { } downButton)
        {
            // downButton.Clicked += (s, e) => {
            //     if (cursorColor == Color.Red) { cursorColor = Color.Green; }
            //     else if (cursorColor == Color.Green) { cursorColor = Color.Blue; }
            //     else { cursorColor = Color.Red; }
            //     DrawLights(ledDisplay);
            // };

            // // With the old system of angle math, it would occasionally drift. This was a way to reset it.
            // downButton.LongClicked += (s, e) => {
            //     angle = new Vector3(0, 0, 0);
            //     // Blink the cursor
            //     for (int i = 0; i < 3; i++)
            //     {
            //         apa102!.Clear();
            //         apa102.Show();
            //         Thread.Sleep(200);
            //         DrawLights(ledDisplay);
            //         Thread.Sleep(200);
            //     }
            // };
        }
        if (projectLab.LeftButton is { } leftButton)
        {
            // leftButton.Clicked += (s, e) => {
            //     cursorLocation -= 1;
            //     if (cursorLocation < 0) { cursorLocation = 0; }
            //     DrawLights(ledDisplay);
            // };
        }
        if (projectLab.RightButton is { } rightButton)
        {
            // rightButton.Clicked += (s, e) => {
            //     cursorLocation += 1;
            //     if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
            //     DrawLights(ledDisplay);
            // };
        }
        // if (projectLab.Gyroscope is { } gyroscope)
        // {
        //     gyroscope.Updated += OnGyroscopeUpdated;
        // }
        if (projectLab.Accelerometer is { } accelerometer)
        {
            accelerometer.Updated += OnAccelerometerUpdated;
        }

        Resolver.Log.Info("Initialization complete");
        return base.Initialize();
    }

    private void OnAccelerometerUpdated(object sender, IChangeResult<Acceleration3D> e) {
        // Resolver.Log.Info($"Accel Gravity: {e.New.X.Gravity}, {e.New.Y.Gravity}, {e.New.Z.Gravity}g");
        
        // if (!handleGyroscope)
        // {
        //     return;
        // }

        var gravityAngle = new Vector3(
            (float) e.New.X.Gravity,
            (float) e.New.Y.Gravity,
            (float) e.New.Z.Gravity
        );

        if (gravityAngle.Y > tiltAngleThreshold)
        {
            // cursorLocation += 1;
            // if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
            // DrawLights(ledDisplay);
            snakeDisplay.MoveRight();
            DrawLights(snakeDisplay);
        }
        else if (gravityAngle.Y < -tiltAngleThreshold)
        {
            // cursorLocation -= 1;
            // if (cursorLocation < 0) { cursorLocation = 0; }
            // DrawLights(ledDisplay);
            snakeDisplay.MoveLeft();
            DrawLights(snakeDisplay);
        }
    }

    // private void OnGyroscopeUpdated(object sender, IChangeResult<AngularVelocity3D> e)
    // {
    //     if (!handleGyroscope)
    //     {
    //         return;
    //     }

    //     // Resolver.Log.Info($"GYRO: {e.New.X.DegreesPerSecond:0.0}, {e.New.Y.DegreesPerSecond:0.0}, {e.New.Z.DegreesPerSecond:0.0}deg/s");
    //     // Get current angle
    //     var newAngle = new Vector3(
    //         angle.X + (float) e.New.X.DegreesPerSecond * (float) updateTime.TotalSeconds, 
    //         angle.Y + (float) e.New.Y.DegreesPerSecond * (float) updateTime.TotalSeconds, 
    //         angle.Z + (float) e.New.Z.DegreesPerSecond * (float) updateTime.TotalSeconds
    //     );

    //     // Define a threshold for changes in the gravity angle
    //     float threshold = 0.1f;

    //     // If the change in the angle is less than the threshold, return early
    //     if (Math.Abs(newAngle.X - angle.X) < threshold)
    //     {
    //         return;
    //     }

    //     // Resolver.Log.Info($"GYRO: {e.New.X.DegreesPerSecond:0.0}, {e.New.Y.DegreesPerSecond:0.0}, {e.New.Z.DegreesPerSecond:0.0}deg/s");
    //     angle = newAngle;
    //     // Resolver.Log.Info($"GYRO: {angle.X:0.0}, {angle.Y:0.0}, {angle.Z:0.0}deg");

    //     if (angle.X > 10)
    //     {
    //         cursorLocation += 1;
    //         if (cursorLocation >= numberOfLeds) { cursorLocation = numberOfLeds - 1; }
    //         DrawLights(ledDisplay);
    //     }
    //     else if (angle.X < -10)
    //     {
    //         cursorLocation -= 1;
    //         if (cursorLocation < 0) { cursorLocation = 0; }
    //         DrawLights(ledDisplay);
    //     }
    // }

    interface ILedDisplay
    {
        // float GetLedBrightness(int ledPosiiton);
        // Color GetLedColor(int ledPosiiton);
        void MoveLeft();
        void MoveRight();
        void DrawDisplay(Apa102 apa102);
        // TODO: ??? Add a method to add a frame to any animations, to allow for "settling" or "momentum" of movements.
        // TODO: ??? Add timing system to keep from speeding up animation just because we are sampling tilt faster.
    }

    class SnakeDisplay : ILedDisplay
    {
        int BodyLength { get; set; }
        public Color SnakeColor { get; set; }
        LinkedList<SnakeSegment> Body { get; set; }
        /// <summary>
        /// Allow for customizing the off color, such as a specific color for the background.
        /// </summary>
        public Color OffColor { get; set; } = Color.Black;
        /// <summary>
        /// Allow for customizing the off brightness, such as a specific brightness for the background.
        /// </summary>
        public float OffBrightness { get; set; } = 0.0f;

        public SnakeDisplay(int bodyLength, Color snakeColor, float maxBrightness)
        {
            BodyLength = bodyLength;
            SnakeColor = snakeColor;
            // Body created with body length items.
            Body = new LinkedList<SnakeSegment>();
            for (int i = 0; i < BodyLength; i++)
            {
                // Start all body segments at initial location.
                // TODO: Adjust brightness based on distance from head.
                Body.AddLast(new SnakeSegment() { SegmentLedIndex = 0, Color = snakeColor, Brightness = maxBrightness });
            }
        }

        // TODO: Show brightness-fading body behind the head, with a length of NumberOfLeds.
        // When the head moves, the tail should move with it, potentially overlapping body segments.
        // Head cannot move off either end of the strip.

        public void MoveLeft()
        {
            var head = Body.First;
            if (head == null) { return; }
            // Move the head one index to the left.
            int newHeadIndex = Body.First.Value.SegmentLedIndex - 1;
            Resolver.Log.Debug($"MoveLeft: newHeadIndex: {newHeadIndex}, numberOfLeds: {numberOfLeds}");
            if (newHeadIndex < 0) {
                // Head is at the end of the strip and won't move.
                // Clamp the new index and trigger the move down the body nodes.
                newHeadIndex = 0;
            }

            // TODO: Left off here with Move failing to happen (0 -> 0)
            head.Move(newHeadIndex);
        }
        public void MoveRight()
        {
            var head = Body.First;
            if (head == null) { return; }
            // Move the head one index to the right.
            int newHeadIndex = Body.First.Value.SegmentLedIndex + 1;
            Resolver.Log.Debug($"MoveRight: newHeadIndex: {newHeadIndex}, numberOfLeds: {numberOfLeds}");
            if (newHeadIndex >= numberOfLeds) {
                // Head is at the end of the strip and won't move.
                // Clamp the new index and trigger the move down the body nodes.
                newHeadIndex = numberOfLeds - 1;
            }

            // TODO: Left off here with Move failing to happen (0 -> 0)
            head.Move(newHeadIndex);
        }
        // TODO: Handle brightness and/or color variation when drawing the body.
        // float GetLedBrightness(int ledPosition)
        // {
        //     return Body.FirstOrDefault(segment => segment.SegmentIndex == ledPosition)?.Brightness ?? OffBrightness;
        // }
        // Color GetLedColor(int ledPosition)
        // {
        //     return Body.FirstOrDefault(segment => segment.SegmentIndex == ledPosition)?.Color ?? OffColor;
        // }
        public void DrawDisplay(Apa102 apa102)
        {
            apa102.Clear();
            // Draw body segments from last to first, so segments closer to head take visual priority (e.g., dimmer tail node overridden by brighter head node).
            var segmentNode = Body.Last;
            while (segmentNode != null)
            {
                float segmentBrightness = segmentNode.Value.Brightness;
                Color segmentColor = segmentNode.Value.Color;
                apa102.SetLed(segmentNode.Value.SegmentLedIndex, segmentColor, segmentBrightness);
                segmentNode = segmentNode.Previous;
            }
            // foreach (var segment in Body.rev)
            // {
            //     float segmentBrightness = segment.Brightness;
            //     Color segmentColor = segment.Color;
            //     apa102.SetLed(segment.SegmentLedIndex, segmentColor, segmentBrightness);
            // }
            apa102.Show();
        }

        public string ToString(int displayLength)
        {
            var textDisplay = new string(' ', displayLength);
            foreach (var segment in Body)
            {
                textDisplay = textDisplay.Remove(segment.SegmentLedIndex, 1).Insert(segment.SegmentLedIndex, "*");
            }
            return $"[{textDisplay}]";
        }
    }
    class RobotEyeDisplay : ILedDisplay
    {
        public RobotEyeDisplay(int numberOfLeds)
        {
            // NumberOfLeds = numberOfTrailLeds;
            TrailLength = numberOfLeds / 3;
            // LedColors = new Color[NumberOfLeds];
            // LedBrightness = new float[NumberOfLeds];
        }

        public void MoveLeft()
        {
            CurrentHeadIndex -= 1;
            if (CurrentHeadIndex < 0) { CurrentHeadIndex = 0; }
        }
        public void MoveRight()
        {
            CurrentHeadIndex += 1;
            if (CurrentHeadIndex >= numberOfLeds) { CurrentHeadIndex = numberOfLeds - 1; }
        }

        public int CurrentHeadIndex { get; private set; }
        public int TrailLength { get; }
        public Color MeteorColor { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="cursorDeltaIndex">
        /// Delta of current LED from current cursor LED location. A `cursorDetlaIndex` of 0 is the cursor, -1 is one behind the cursor, 2 is two ahead of the cursor, etc.
        /// </param>
        float GetLedBrightness(int ledPosiiton)
        {
            int cursorDeltaIndex = ledPosiiton - CurrentHeadIndex;
            if (cursorDeltaIndex == 0)
            {
                return 1.0f;
            }
            else if (cursorDeltaIndex > TrailLength)
            {
                return 0.0f;
            }
            else {
                // For Meteor Trail, we want the cursor to be the brightest, and then trail off behind the cursor.
                return 1.0f / (float)Math.Pow(2, Math.Abs(cursorDeltaIndex));
            }
        }
        Color GetLedColor(int ledPosiiton)
        {
            return MeteorColor;
        }
        public void DrawDisplay(Apa102 apa102)
        {
            apa102.Clear();
            for (int i = 0; i < numberOfLeds; i++)
            {
                var brightness = GetLedBrightness(i);
                var color = GetLedColor(i);
                apa102.SetLed(i, color, brightness);
            }
            apa102.Show();
        }
    }

    void DrawLights(ILedDisplay ledDisplay)
    {
        Resolver.Log.Info((ledDisplay as SnakeDisplay)?.ToString(apa102.NumberOfLeds));
        ledDisplay.DrawDisplay(apa102);
        // // Resolver.Log.Trace($"ShowCursor: {cursorLocation}");
        // apa102!.Clear();
        // // apa102.SetLed(cursorLocation, cursorColor);
        // for (int i = 0; i < numberOfLeds; i++)
        // {
        //     // var brightness = ledDisplay.GetLedBrightness(i - cursorLocation);
        //     // apa102.SetLed(i, cursorColor, brightness);
        //     var color = snakeDisplay.GetLedColor(i);
        //     var brightness = snakeDisplay.GetLedBrightness(i);
        //     apa102.SetLed(i, color, brightness);
        // }
        // apa102.Show();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        if (projectLab!.Gyroscope is { } gyroscope)
        {
            gyroscope.StartUpdating(sensorUpdateTime);
        }
        // displayController?.Update();

        Resolver.Log.Info("starting blink");

        DrawLights(snakeDisplay);

        return base.Run();
    }
}

class SnakeSegment
{
    public int SegmentLedIndex { get; set; }
    public Color Color { get; set; }
    public float Brightness { get; set; }
}
static class SnakeSegmentExtensions
{
    public static void Move(this LinkedListNode<SnakeSegment> segmentNode, int newIndexToMoveTo)
    {
        if (segmentNode == null)
        {
            // Shouldn't happen[?], but called on null node.
            return;
        }
        if (segmentNode.Value.SegmentLedIndex == newIndexToMoveTo)
        {
            // Segment is already at index.
            // Try to move next segment to this index instead.
            // If next node is null, then we're done.
            segmentNode.Next?.Move(newIndexToMoveTo);
        }

        int indexBeingVacated = segmentNode.Value.SegmentLedIndex;
        Resolver.Log.Debug($"Move: {indexBeingVacated} -> {newIndexToMoveTo}");
        segmentNode.Value.SegmentLedIndex = newIndexToMoveTo;
        segmentNode.Next?.Move(indexBeingVacated);
    }
}