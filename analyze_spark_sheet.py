#!/usr/bin/env python3
"""
Spark Sheet Analyzer

This script analyzes the spark-sheet.png to understand its structure
and determine the correct animation parameters.
"""

from PIL import Image
import numpy as np

def analyze_spark_sheet():
    # Path to the spark sheet
    spark_path = "Projektarbeit/assets/animation/bolt/spark-sheet.png"
    
    try:
        # Load the image
        img = Image.open(spark_path)
        width, height = img.size
        
        print("Spark Sheet Analysis")
        print("===================")
        print(f"Image dimensions: {width}x{height}")
        print(f"Mode: {img.mode}")
        
        # Convert to RGBA if needed
        if img.mode != 'RGBA':
            img = img.convert('RGBA')
        
        # Convert to numpy array for analysis
        img_array = np.array(img)
        
        # Find non-transparent regions to understand frame layout
        alpha_channel = img_array[:, :, 3]
        non_transparent = alpha_channel > 0
        
        # Find bounding box of all content
        rows = np.any(non_transparent, axis=1)
        cols = np.any(non_transparent, axis=0)
        
        if np.any(rows) and np.any(cols):
            rmin, rmax = np.where(rows)[0][[0, -1]]
            cmin, cmax = np.where(cols)[0][[0, -1]]
            
            content_width = cmax - cmin + 1
            content_height = rmax - rmin + 1
            
            print(f"Content bounding box: {content_width}x{content_height}")
            print(f"Content position: ({cmin}, {rmin}) to ({cmax}, {rmax})")
        
        # Analyze potential frame divisions
        print("\nFrame Analysis:")
        
        # User mentioned 64x32 per frame, let's check if this matches
        frame_width = 64
        frame_height = 32
        
        possible_cols = width // frame_width
        possible_rows = height // frame_height
        
        print(f"With 64x32 frames: {possible_cols} columns, {possible_rows} rows")
        print(f"Total possible frames: {possible_cols * possible_rows}")
        
        # Check for other common frame sizes
        common_sizes = [(32, 32), (64, 64), (128, 32), (32, 64)]
        for fw, fh in common_sizes:
            cols = width // fw
            rows = height // fh
            if cols > 0 and rows > 0:
                print(f"With {fw}x{fh} frames: {cols} columns, {rows} rows = {cols * rows} frames")
        
        # Analyze the actual layout by looking for content gaps
        print("\nAnalyzing content distribution...")
        
        # Sample some potential frame boundaries
        if width >= 64:
            print("Checking vertical divisions at 64-pixel intervals:")
            for i in range(0, width, 64):
                if i + 64 <= width:
                    frame_section = non_transparent[:, i:i+64]
                    has_content = np.any(frame_section)
                    content_density = np.sum(frame_section) / (64 * height) if has_content else 0
                    print(f"  Frame at x={i}-{i+64}: {'Content' if has_content else 'Empty'} (density: {content_density:.3f})")
        
        # Check if the image is more likely horizontal or vertical sprite sheet
        if width > height:
            print(f"\nLikely horizontal sprite sheet (width={width} > height={height})")
            recommended_layout = "horizontal"
        else:
            print(f"\nLikely vertical sprite sheet (height={height} > width={width})")
            recommended_layout = "vertical"
        
        # Generate recommendations
        print("\nRecommendations:")
        print("================")
        
        if width == 256 and height == 32:
            print("Detected: 256x32 image")
            print("Likely: 4 frames of 64x32 each (horizontal layout)")
            print("Animation data should be: (1, 4, ...) for 1 row, 4 columns")
        elif width == 128 and height == 64:
            print("Detected: 128x64 image")
            print("Likely: 2x2 grid = 4 frames of 64x32 each")
            print("Animation data should be: (2, 2, ...) for 2 rows, 2 columns")
        elif width == 64 and height == 128:
            print("Detected: 64x128 image")
            print("Likely: 4 frames of 64x32 each (vertical layout)")
            print("Animation data should be: (4, 1, ...) for 4 rows, 1 column")
        else:
            # Best guess based on 64x32 frame size
            cols = max(1, width // 64)
            rows = max(1, height // 32)
            print(f"Best guess: {rows} rows, {cols} columns")
            print(f"Animation data should be: ({rows}, {cols}, ...)")
        
        # Check orientation of content
        print("\nOrientation Analysis:")
        if np.any(non_transparent):
            # Find the primary direction of content
            center_y = height // 2
            center_x = width // 2
            
            # Check horizontal vs vertical extent
            horizontal_extent = cmax - cmin if np.any(cols) else 0
            vertical_extent = rmax - rmin if np.any(rows) else 0
            
            if horizontal_extent > vertical_extent:
                print("Content appears to be horizontally oriented")
                print("Sprite rotation may need adjustment for vertical movement")
            else:
                print("Content appears to be vertically oriented")
                print("Sprite should work well for vertical movement")
        
    except Exception as e:
        print(f"Error analyzing spark sheet: {e}")
        return None

if __name__ == "__main__":
    analyze_spark_sheet()