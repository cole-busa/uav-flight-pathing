import seaborn as sns
import matplotlib.pyplot as plt

# Uninformed Scenario Figure
# Create a list of scenario names
scenario_names = ["Solo", "Origin", "Corners", "Random", "Quadrant"]

# Create a list of average move counts
average_move_counts = [687, 220, 165, 206, 185]

# Create a color palette
palette = sns.color_palette("hls", len(scenario_names))

# Create a bar graph
sns.barplot(x=scenario_names, y=average_move_counts, palette=palette)

# Add a title and labels
plt.title("Uninformed Scenarios' Average Move Count")
plt.xlabel("Scenario Name")
plt.ylabel("Average Move Count")

# Show the plot
plt.show()


# Informed Scenario Figures
# With Badly Informed
# Create a list of scenario names
scenario_names = ["Manhattan", "Euclidean", "Perfectly", "Decently", "Badly"]

# Create a list of average move counts
average_move_counts = [34, 32, 16, 33, 853]

# Create a color palette
palette = sns.color_palette("hls", len(scenario_names))

# Create a bar graph
sns.barplot(x=scenario_names, y=average_move_counts, palette=palette)

# Add a title and labels
plt.title("Informed Scenarios' Average Move Count")
plt.xlabel("Scenario Name")
plt.ylabel("Average Move Count")

# Show the plot
plt.show()

# Without Badly Informed
# Create a list of scenario names
scenario_names = ["Manhattan", "Euclidean", "Perfectly", "Decently"]

# Create a list of average move counts
average_move_counts = [34, 32, 16, 33]

# Create a color palette
palette = sns.color_palette("hls", len(scenario_names))

# Create a bar graph
sns.barplot(x=scenario_names, y=average_move_counts, palette=palette)

# Add a title and labels
plt.title("Informed Scenarios' Average Move Count")
plt.xlabel("Scenario Name")
plt.ylabel("Average Move Count")

# Show the plot
plt.show()


# Moving Goal Scenario Figure
# Create a list of scenario names
scenario_names = ["Moving Goal", "Quadrant Limited", "Information Decay"]

# Create a list of average move counts
average_move_counts = [201, 176, 135]

# Create a color palette
palette = sns.color_palette("hls", len(scenario_names))

# Create a bar graph
sns.barplot(x=scenario_names, y=average_move_counts, palette=palette)

# Add a title and labels
plt.title("Moving Goal Scenarios' Average Move Count")
plt.xlabel("Scenario Name")
plt.ylabel("Average Move Count")

# Show the plot
plt.show()