resource "kubernetes_deployment" "trade_engine" {
  metadata {
    name   = "trade-engine"
    labels = { app = "trade-engine" }
  }

  spec {
    replicas = 1
    selector {
      match_labels = { app = "trade-engine" }
    }
    template {
      metadata {
        labels = { app = "trade-engine" }
      }
      spec {
        container {
          image             = "trade-engine:v1"
          name              = "engine"
          image_pull_policy = "Never"

          env {
            name  = "ConnectionStrings__DefaultConnection"
            value = "Host=trade-db;Database=${local.db_name};Username=${local.db_user};Password=${local.db_pass}"
          }
          port { container_port = 8080 }
        }
      }
    }
  }
}