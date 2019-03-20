git config --global credential.helper store
Add-Content "$HOME\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
git config --global user.email "appveyor@example.com"
git config --global user.name "AppVeyor"
git add --all .
git commit -m "AppVeyor"
git push