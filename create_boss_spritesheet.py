#!/usr/bin/env python3
"""
Boss Animation Sprite Sheet Creator

This script combines the 80 individual boss animation frames (0001.png to 0080.png)
into a single sprite sheet that can be used by the game's animation system.

The sprite sheet will be arranged in a 10x8 grid (10 columns, 8 rows = 80 frames)
which matches the animation_data configuration in the Boss.cs file.
"""

import os
from PIL import Image
import sys

def create_boss_spritesheet():
    # Define paths
    frames_dir = "DropDown/assets/animation/boss"
    output_path = os.path.join(frames_dir, "boss_spritesheet.png")
    
    # Check if frames directory exists
    if not os.path.exists(frames_dir):
        print(f"Error: Boss animation directory not found: {frames_dir}")
        return False
    
    # Get list of frame files
    frame_files = []
    for i in range(1, 81):  # 0001.png to 0080.png
        filename = f"{i:04d}.png"
        filepath = os.path.join(frames_dir, filename)
        if os.path.exists(filepath):
            frame_files.append(filepath)
        else:
            print(f"Warning: Frame file not found: {filename}")
    
    if len(frame_files) == 0:
        print("Error: No boss animation frames found!")
        return False
    
    print(f"Found {len(frame_files)} boss animation frames")
    
    # Load the first frame to get dimensions
    try:
        first_frame = Image.open(frame_files[0])
        frame_width, frame_height = first_frame.size
        first_frame.close()
    except Exception as e:
        print(f"Error loading first frame: {e}")
        return False
    
    print(f"Frame dimensions: {frame_width}x{frame_height}")
    
    # Calculate sprite sheet dimensions
    cols = 10
    rows = 8
    sheet_width = frame_width * cols
    sheet_height = frame_height * rows
    
    print(f"Creating sprite sheet: {sheet_width}x{sheet_height} ({cols}x{rows} grid)")
    
    # Create the sprite sheet
    try:
        sprite_sheet = Image.new('RGBA', (sheet_width, sheet_height), (0, 0, 0, 0))
        
        for i, frame_path in enumerate(frame_files):
            if i >= 80:  # Safety check
                break
                
            # Calculate position in grid
            col = i % cols
            row = i // cols
            x = col * frame_width
            y = row * frame_height
            
            # Load and paste frame
            frame = Image.open(frame_path)
            sprite_sheet.paste(frame, (x, y))
            frame.close()
            
            if (i + 1) % 10 == 0:
                print(f"Processed {i + 1}/80 frames...")
        
        # Save the sprite sheet
        sprite_sheet.save(output_path, "PNG")
        sprite_sheet.close()
        
        print(f"Boss sprite sheet created successfully: {output_path}")
        print("The game will now use the new 80-frame boss animation!")
        return True
        
    except Exception as e:
        print(f"Error creating sprite sheet: {e}")
        return False

def main():
    print("Boss Animation Sprite Sheet Creator")
    print("===================================")
    
    if create_boss_spritesheet():
        print("\nSuccess! The boss sprite sheet has been created.")
        print("Run the game to see the new boss animation in action.")
    else:
        print("\nFailed to create boss sprite sheet.")
        print("Please check that the boss animation frames exist in:")
        print("DropDown/assets/animation/boss/")
        print("Files should be named 0001.png through 0080.png")

if __name__ == "__main__":
    main()