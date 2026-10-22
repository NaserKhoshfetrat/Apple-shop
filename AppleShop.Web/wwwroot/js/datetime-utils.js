/**
 * Sample: convertUtcDatesToLocal
 * 
                        <td class="text-nowrap text-truncate" style="max-width: 150px;">
                            <span class="local-datetime" data-utc="@item.CreatedAt.ToString("o")">
                                @item.CreatedAt.ToString("MMMM dd, yyyy h:mm tt") utc-time  <!-- fallback for noscript -->
                            </span>
                        </td>



 * Sample: convertUtcDatesToTimezone
    Also you need to set the IANA timezone names like: convertUtcDatesToTimezone('America/New_York', '.country-datetime');

                        <td>
                            <span class="country-datetime" data-utc="@item.CreatedAt.ToString("o")"></span>
                        </td>
    

 */


// Run after DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    // Show in client's local time
    convertUtcDatesToLocal();
            
    // show in USA Eastern Time
    convertUtcDatesToTimezone('America/New_York', '.country-datetime');
});


// Convert a UTC date string to a specific timezone
function convertUtcToTimezone(utcDateString, timezone) {
    const date = new Date(utcDateString);
    if (isNaN(date)) return '';
    
    return date.toLocaleString(undefined, {
        timeZone: timezone,
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: 'numeric',
        minute: '2-digit',
        hour12: true
    });
}

// Convert all .local-datetime elements to client's local time (original)
function convertUtcDatesToLocal() {
    document.querySelectorAll('.local-datetime').forEach(el => {
        const utcDate = el.getAttribute('data-utc');
        const localString = convertUtcToTimezone(utcDate, Intl.DateTimeFormat().resolvedOptions().timeZone);
        if (localString) el.textContent = localString;
    });
}

// NEW: Convert all elements with a specific class to a fixed country timezone
function convertUtcDatesToTimezone(timezone, selector = '.country-datetime') {
    document.querySelectorAll(selector).forEach(el => {
        const utcDate = el.getAttribute('data-utc');
        const tzString = convertUtcToTimezone(utcDate, timezone);
        if (tzString) el.textContent = tzString;
    });
} 