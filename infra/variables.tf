# This block reads the .env file from the root
locals {
  # Read the .env from the root, split by lines, and filter out comments/empty lines
  env_content = file("${path.module}/../.env")
  env_lines   = [for l in split("\n", local.env_content) : l if length(split("=", l)) == 2 && !startswith(l, "#")]
  
  # Create a Map from the .env file
  env_vars = { for l in local.env_lines : trimspace(split("=", l)[0]) => trimspace(split("=", l)[1]) }

  # Extract the specific variables
  db_user = local.env_vars["POSTGRES_USER"]
  db_pass = local.env_vars["POSTGRES_PASSWORD"]
  db_name = local.env_vars["POSTGRES_DB"]
}