## Project: Light-up hoodie

Feed LED strip through the hood drawstring path, connect to power and microcontroller. Offer buttons to control the light pattern and brightness.

### Components

* LED strip (WS2812B or APA102)
* Microcontroller (Meadow F7 Feather for size, or custom PCB with Meadow Core-Compute Module)
* Battery (USB power bank to Meadow _and_ driving LEDs, or USB C PD board to convert to 5V, split for LEDs and Meadow)
  * Wingman swappable battery pack ordered (20240614).
* Buttons (3x for pattern change, brightness up, brightness down)
* Hoodie with a drawstring (already have zip-up one)
  * Drawstring channel is ~800mm from end to end and ~25mm wide
  * Hood peak perimeter is ~590mm from tip to neck seam
  * From neck seam to top of waist seam is ~620mm
  * Could sew a pocket into back of hoodie (neck or inside normal pocket) to hold battery and microcontroller

### Considerations

* Current draw from LEDs cannot exceed microcontroller 5V pin output
* Determine estimated draw of 80cm of LEDs at X density (current test strip is 30 LEDs/m, but 60 and 144 are also available)
  * Product sheet seems to indicate 0.3W per LED
  * A = W / V => 0.3W / 5V = 0.06A per LED
  * 30 LEDs/m is 9W/m => LEDs for 80cm is 7.2W => 1.44A
  * 60 LEDs/m is 18W/m => LEDs for 80cm is 14.4W => 2.88A

### Plan

1. Acquire 1M of 60 LEDs/m strip
1. Determine way to split power bank power to Meadow and LEDs
1. Identify newest Meadow Feather board
1. Power from USB power bank, split to Meadow and LEDs.
1. Cut drawstring stitch at top to allow full drawstring path use.
1. Cut small portion of seam at the drawstring grommet to allow LED strip to pass through.
