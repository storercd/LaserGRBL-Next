# Example script that runs when a job completes
# This is called automatically by LaserGRBL Storer Edition

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logFile = Join-Path $PSScriptRoot "job-completions.log"

# Log the completion
Add-Content -Path $logFile -Value "$timestamp - Job completed!"

# ===== NTFY.SH PHONE NOTIFICATION =====
# Subscribe to this topic in your ntfy app: lasergrbl-notifications-YOUR_UNIQUE_ID
# Replace YOUR_UNIQUE_ID with something random to keep your topic private
$ntfyTopic = "lasergrbl-notifications-zanemcfate"  # Change this to something unique!
$ntfyServer = "https://ntfy.sh"

try {
    $body = "Your laser job completed at $timestamp. Machine is ready for the next job!"
    
    $headers = @{
        "Title" = "LaserGRBL Job Complete"
        "Priority" = "high"  # High priority - will make sound even in Do Not Disturb
        "Tags" = "white_check_mark,fire"  # Emojis in notification
    }
    
    Invoke-RestMethod -Uri "$ntfyServer/$ntfyTopic" -Method Post -Body $body -Headers $headers -ContentType "text/plain"
    Add-Content -Path $logFile -Value "$timestamp - Notification sent successfully"
} catch {
    Add-Content -Path $logFile -Value "$timestamp - Failed to send notification: $_"
}
