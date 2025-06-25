#!/usr/bin/env python3
"""
Boss Sprite Sheet Generator
Combines 80 individual boss animation frames into a single sprite sheet.
"""

import os
import sys
from PIL import Image
import math

def create_boss_spritesheet():
    """Create a sprite sheet from 80 individual boss animation frames."""
    
    # Configuration
    input_dir = "Projektarbeit/assets/animation/enemy/boss"
    output_file = "Projektarbeit/assets/animation/enemy/boss_spritesheet.png"
    total_frames = 80
    
    # Calculate grid dimensions for 80 frames
    # We want a reasonable aspect ratio, so let's use 10 columns and 8 rows
    cols = 10
    rows = 8
    
    print(f"Creating boss sprite sheet with {total_frames} frames in a {cols}x{rows} grid...")
    
    # Check if input directory exists
    if not os.path.exists(input_dir):
        print(f"Error: Input directory '{input_dir}' does not exist!")
        return False
    
    # Get all frame files
    frame_files = []
    for i in range(1, total_frames + 1):
        frame_path = os.path.join(input_dir, f"{i:04d}.png")
        if os.path.exists(frame_path):
            frame_files.append(frame_path)
        else:
            print(f"Warning: Frame {frame_path} not found!")
    
    if not frame_files:
        print("Error: No frame files found!")
        return False
    
    print(f"Found {len(frame_files)} frame files")
    
    # Load first image to get dimensions
    try:
        first_image = Image.open(frame_files[0])
        frame_width, frame_height = first_image.size
        first_image.close()
    except Exception as e:
        print(f"Error loading first frame: {e}")
        return False
    
    print(f"Frame dimensions: {frame_width}x{frame_height}")
    
    # Calculate sprite sheet dimensions
    sheet_width = frame_width * cols
    sheet_height = frame_height * rows
    
    print(f"Sprite sheet dimensions: {sheet_width}x{sheet_height}")
    
    # Create the sprite sheet
    try:
        # Create a new image with RGBA mode to support transparency
        spritesheet = Image.new('RGBA', (sheet_width, sheet_height), (0, 0, 0, 0))
        
        # Place each frame in the grid
        for i, frame_path in enumerate(frame_files):
            if i >= total_frames:
                break
                
            # Calculate position in grid
            col = i % cols
            row = i // cols
            
            # Calculate pixel position
            x = col * frame_width
            y = row * frame_height
            
            # Load and paste the frame
            try:
                frame_image = Image.open(frame_path)
                
                # Convert to RGBA if needed
                if frame_image.mode != 'RGBA':
                    frame_image = frame_image.convert('RGBA')
                
                spritesheet.paste(frame_image, (x, y))
                frame_image.close()
                
                print(f"Added frame {i+1:02d} at position ({col}, {row})")
                
            except Exception as e:
                print(f"Error processing frame {frame_path}: {e}")
                continue
        
        # Create output directory if it doesn't exist
        os.makedirs(os.path.dirname(output_file), exist_ok=True)
        
        # Save the sprite sheet
        spritesheet.save(output_file, 'PNG')
        spritesheet.close()
        
        print(f"\nSuccessfully created boss sprite sheet: {output_file}")
        print(f"Grid layout: {cols} columns x {rows} rows")
        print(f"Total frames: {total_frames}")
        print(f"Frame dimensions: {frame_width}x{frame_height}")
        print(f"Sprite sheet dimensions: {sheet_width}x{sheet_height}")
        print(f"Animation will play at 15 FPS as configured in the game")
        
        return True
        
    except Exception as e:
        print(f"Error creating sprite sheet: {e}")
        return False

def main():
    """Main function."""
    print("Boss Sprite Sheet Generator")
    print("=" * 40)
    
    success = create_boss_spritesheet()
    
    if success:
        print("\n✅ Boss sprite sheet created successfully!")
        print("You can now use this in your game with the animation data:")
        print("animation_data('assets/animation/enemy/boss_spritesheet.png', 8, 10, true, true, 15, true)")
    else:
        print("\n❌ Failed to create boss sprite sheet!")
        sys.exit(1)

if __name__ == "__main__":
    main()